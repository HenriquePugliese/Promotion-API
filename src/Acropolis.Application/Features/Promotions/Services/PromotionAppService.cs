using Acropolis.Application.Base.Models;
using Acropolis.Application.Base.Persistence;
using Acropolis.Application.Base.Services;
using Acropolis.Application.Features.Promotions.Repositories;
using Acropolis.Application.Features.Promotions.Requests;
using Acropolis.Application.Features.Promotions.Services.Contracts;
using Acropolis.Application.Base.Notifications;
using Acropolis.Application.Features.Promotions.Responses;
using Acropolis.Application.Features.Promotions.Validators;
using Acropolis.Application.Base.Extensions;

namespace Acropolis.Application.Features.Promotions.Services;

public class PromotionAppService : AppService, IPromotionAppService
{
    private readonly IPromotionRepository _repository;
    private readonly IPromotionCnpjRepository _promotionCnpjRepository;
    private const string promotionNotFoundError = "Promotion not found";
    private const string notificationErrorCode = "Promotion";

    public PromotionAppService(IUnitOfWork unitOfWork, IPromotionRepository repository, IPromotionCnpjRepository promotionCnpjRepository)
        : base(unitOfWork)
    {
        _repository = repository;
        _promotionCnpjRepository = promotionCnpjRepository;
    }

    public async Task<(Response, Promotion?)> CreatePromotionAsync(CreatePromotionRequest request, Guid sellerId, CancellationToken cancellationToken = default)
    {
        var promotion = new Promotion(request, sellerId);
        var (isPromotionValid, promotionErrorMessage) = await ValidatePromotion(promotion);

        if (!isPromotionValid)
            return (promotionErrorMessage, null);

        await _repository.AddAsync(promotion);
        await Commit();

        return (Response.Valid(), promotion);
    }

    public async Task<Response> RemovePromotionAsync(RemovePromotionRequest request, Guid sellerId, CancellationToken cancellationToken = default)
    {
        if(string.IsNullOrEmpty(request.ExternalId))
            return Response.Invalid(notificationErrorCode, "Identificador externo da promoção é obrigatório.");

        var promotion = await _repository.GetByExternalIdAsync(request.ExternalId, sellerId);

        if (promotion is null)
            return Response.Invalid(notificationErrorCode, promotionNotFoundError);

        _repository.Remove(promotion);

        await Commit();

        return Response.Valid();
    }

    public async Task<(Response, PromotionCnpjResponse?)> AddCnpjsAsync(CreatePromotionCnpjRequest request, Guid sellerId, CancellationToken cancellationToken = default)
    {
        var (isPromotionCnpjsValid, promotionCnpjsErrorMessage) = ValidatePromotionCnpjs(request);

        if (!isPromotionCnpjsValid)
            return (promotionCnpjsErrorMessage, null);
        
        var promotion = await _repository.GetByExternalIdAsync(request.ExternalId, sellerId);
        var promotionCnpjsExisting = (await _promotionCnpjRepository.FindAsync(new PromotionCnpjParameters() { PageSize = request.Cnpjs.Count(), ExternalId = request.ExternalId, Cnpjs = request.Cnpjs }, sellerId)).Data.SelectMany(p => p.Cnpjs);
        var promotionCnpjs = request.Cnpjs.AsEnumerable().Select(cnpj => new PromotionCnpj(request.ExternalId, cnpj, sellerId)).Where(p => !promotionCnpjsExisting.Contains(p.Cnpj)).ToList();

        if (promotion is null)
            return (Response.Invalid(notificationErrorCode, promotionNotFoundError), null);

        if (promotion.Cnpjs is null)
            promotion.Cnpjs = new List<PromotionCnpj>();

        promotion.Cnpjs.AddRange(promotionCnpjs);
        
        await Commit();
       
        var addedCnpjs = promotionCnpjs?.Select(promotionCnpj => promotionCnpj.Cnpj);
        var promotionCnpjsResponse = new PromotionCnpjResponse($"{promotion?.ExternalId}", addedCnpjs);

        return (Response.Valid(), promotionCnpjsResponse);
    }

    public async Task<Response> RemoveCnpjAsync(RemovePromotionCnpjRequest request, Guid sellerId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(request.ExternalId))
            return Response.Invalid(notificationErrorCode, "Identificador externo da promoção é obrigatório.");

        if (!CnpjValidator.Validate(request.Cnpj))
            return Response.Invalid(notificationErrorCode, "CNPJ inválido.");

        var promotionCnpj = await _promotionCnpjRepository.GetByAsync(request.ExternalId, request.Cnpj.OnlyNumbers(), sellerId);

        if (promotionCnpj is null)
            return Response.Invalid(notificationErrorCode, "Promoção com CNPJ informado é inexistente.");

        _promotionCnpjRepository.Remove(promotionCnpj);

        await Commit();

        return Response.Valid();
    }

    private static (bool IsValid, Response Message) ValidatePromotionCnpjs(CreatePromotionCnpjRequest request)
    {
        var notificationErrors = new List<Notification>();
        var cnpjsMaxAmount = 1000;

        if (string.IsNullOrWhiteSpace(request.ExternalId))
            notificationErrors.Add(new(notificationErrorCode, "Identificador externo da promoção é obrigatório."));

        if (request.Cnpjs is null || !request.Cnpjs.Any())
        {
            notificationErrors.Add(new(notificationErrorCode, "No mínimo 1 CNPJ é obrigatório."));
            return (false, Response.Invalid(notificationErrors));
        }
        else if (request.Cnpjs.Count() > cnpjsMaxAmount)
            notificationErrors.Add(new(notificationErrorCode, $"No máximo {cnpjsMaxAmount} CNPJs são permitidos."));

        if (!request.Cnpjs.AsEnumerable().All(cnpj => CnpjValidator.Validate(cnpj)))
            notificationErrors.Add(new(notificationErrorCode, "CNPJs inválidos."));

        return notificationErrors.Any() ? (false, Response.Invalid(notificationErrors)) : (true, Response.Valid());
    }

    private async Task<(bool IsValid, Response Message)> ValidatePromotion(Promotion promotion)
    {
        var notificationErrors = new List<Notification>();
        var percentageRuleMin = 0;
        var percentageRuleMax = 100;

        if (string.IsNullOrWhiteSpace(promotion.Name))
            notificationErrors.Add(new(notificationErrorCode, "Nome da promoção é obrigatório."));
        
        if (promotion.DtEnd < DateTime.Today)
            notificationErrors.Add(new(notificationErrorCode, "A data final da promoção não pode ser menor do que a data atual."));

        if (promotion.Attributes?.Any() == false)
            notificationErrors.Add(new(notificationErrorCode, "Ao menos um atributo da promoção é obrigatório."));

        if (promotion.Rules?.Any() == false)
            notificationErrors.Add(new(notificationErrorCode, "Ao menos uma regra da promoção é obrigatório."));
        else if ((promotion.Rules?.Any() == true) && (promotion.Rules.Any(promotionRule => promotionRule.Percentage < percentageRuleMin || promotionRule.Percentage > percentageRuleMax)))
            notificationErrors.Add(new(notificationErrorCode, "Regra de percentual da promoção é inválida."));

        if (promotion.DtStart == DateTime.MinValue || promotion.DtEnd == DateTime.MinValue)
            notificationErrors.Add(new(notificationErrorCode, "Datas da promoção são obrigatórias."));

        if (string.IsNullOrWhiteSpace(promotion.ExternalId))
            notificationErrors.Add(new(notificationErrorCode, "Identificador externo da promoção é obrigatório."));
        else if (await _repository.HasPromotionAsync(promotion.ExternalId))
            notificationErrors.Add(new(notificationErrorCode, $"Já existe promoção cadastrada com identificador externo : {promotion.ExternalId}."));

        return notificationErrors.Any() ? (false, Response.Invalid(notificationErrors)) : (true, Response.Valid());
    }

    public async Task<(Response, PromotionResponse?)> GetPromotionAsync(string externalId, Guid sellerId)
    {
        var promotion = await _repository.GetByExternalIdAsync(externalId, sellerId);

        if (promotion is null)
            return (Response.Invalid(notificationErrorCode, promotionNotFoundError), null);

        var promotionResponse = new PromotionResponse(promotion);

        return (Response.Valid(), promotionResponse);
    }
}
