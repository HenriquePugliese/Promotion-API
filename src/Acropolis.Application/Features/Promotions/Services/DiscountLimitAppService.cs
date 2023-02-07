using Acropolis.Application.Base.Models;
using Acropolis.Application.Base.Notifications;
using Acropolis.Application.Base.Persistence;
using Acropolis.Application.Base.Services;
using Acropolis.Application.Features.Promotions.Repositories;
using Acropolis.Application.Features.Promotions.Requests;
using Acropolis.Application.Features.Promotions.Responses;
using Acropolis.Application.Features.Promotions.Services.Contracts;
using Acropolis.Application.Features.Promotions.Validators;

namespace Acropolis.Application.Features.Promotions.Services;

public class DiscountLimitAppService : AppService, IDiscountLimitAppService
{
    private readonly IPromotionCnpjDiscountLimitRepository _repository;

    private const string discountLimitNotFoundError = "Limite de desconto não encontrado.";
    private const string notificationErrorCode = "DiscountLimit";

    public DiscountLimitAppService(IUnitOfWork unitOfWork, IPromotionCnpjDiscountLimitRepository repository)
        : base(unitOfWork)
    {
        _repository = repository;
    }

    public async Task<(Response, IEnumerable<PromotionCnpjDiscountLimit>?)> CreateDiscountLimitAsync(IEnumerable<CreateDiscountLimitRequest> request, Guid sellerId, CancellationToken cancellationToken = default)
    {
        var discountLimits = request.Select(discountLimitRequest => new PromotionCnpjDiscountLimit(discountLimitRequest.Cnpj, discountLimitRequest.Percent, sellerId));
        var (isDiscountLimitValid, discountLimitErrorMessage) = ValidateDiscountLimits(discountLimits);

        if (!isDiscountLimitValid)
            return (discountLimitErrorMessage, null);

        var cnpjsWithDiscountLimits = await _repository.GetObjectsCnpjsWithDiscountLimitsAsync(discountLimits.Select(discountLimit => discountLimit.Cnpj), sellerId);

        foreach (var discountLimit in discountLimits)
        {
            var discountLimitExists = cnpjsWithDiscountLimits.FirstOrDefault(c => c.Cnpj == discountLimit.Cnpj && c.SellerId == discountLimit.SellerId);
            if (discountLimitExists != null)
            {
                discountLimitExists.UpdatePercent(discountLimit.Percent);
                _repository.Update(discountLimitExists);
            }
            else
                await _repository.AddAsync(discountLimit);
        }

        await Commit();

        return (Response.Valid(), discountLimits);
    }

    public async Task<Response> RemoveDiscountLimitAsync(RemoveDiscountLimitRequest request, Guid sellerId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(request.Cnpj))
            return Response.Invalid(notificationErrorCode, "CNPJ é obrigatório.");

        var discountLimit = await _repository.GetByCnpjAsync(request.Cnpj, sellerId);

        if (discountLimit is null)
            return Response.Invalid(notificationErrorCode, discountLimitNotFoundError);

        _repository.Remove(discountLimit);

        await Commit();

        return Response.Valid();
    }

    private static (bool IsValid, Response Message) ValidateDiscountLimits(IEnumerable<PromotionCnpjDiscountLimit> discountLimits)
    {
        var notificationErrors = new List<Notification>();
        var cnpjsMaxAmount = 50;
        var percentageMin = 0;
        var percentageMax = 100;

        if (!discountLimits.Any())
            notificationErrors.Add(new(notificationErrorCode, $"No mínimo um percentual de desconto é obrigatório."));

        if (discountLimits.Count() > cnpjsMaxAmount)
            notificationErrors.Add(new(notificationErrorCode, $"No máximo {cnpjsMaxAmount} CNPJs são permitidos."));

        foreach (var discountLimit in discountLimits)
        {
            if (discountLimit.Percent <= percentageMin)
                notificationErrors.Add(new(notificationErrorCode, $"Valor do percentual de desconto deve ser maior que '{percentageMin}'."));

            if (discountLimit.Percent > percentageMax)
                notificationErrors.Add(new(notificationErrorCode, $"Valor do percentual de desconto deve ser menor que '{percentageMax}'."));

            if (string.IsNullOrWhiteSpace(discountLimit.Cnpj))
                notificationErrors.Add(new(notificationErrorCode, "CNPJ é obrigatório."));

            if (!CnpjValidator.Validate(discountLimit.Cnpj))
                notificationErrors.Add(new(notificationErrorCode, $"CNPJ '{discountLimit.RawCnpj}' inválido."));
        }

        return notificationErrors.Any() ? (false, Response.Invalid(notificationErrors)) : (true, Response.Valid());
    }

    public async Task<(Response, DiscountLimitResponse?)> GetDiscountLimitAsync(string cnpj, Guid sellerId)
    {
        var discountLimit = await _repository.GetByCnpjAsync(cnpj, sellerId);

        if (discountLimit is null)
            return (Response.Invalid(notificationErrorCode, discountLimitNotFoundError), null);

        var discountLimitResponse = new DiscountLimitResponse(discountLimit);

        return (Response.Valid(), discountLimitResponse);
    }
}
