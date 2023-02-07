using Acropolis.Application.Base.Extensions;
using Acropolis.Application.Base.Notifications;
using Acropolis.Application.Base.Persistence;
using Acropolis.Application.Features.Customers.Repositories;
using Acropolis.Application.Features.Promotions;
using Acropolis.Application.Features.Promotions.Repositories;

namespace Acropolis.Application.Base.Services;

public abstract class AppService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPromotionRepository? _promotionRepository;
    private readonly IPromotionCnpjRepository? _promotionCnpjRepository;
    private readonly ICustomerRepository? _customerRepository;

    protected AppService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    protected AppService(IUnitOfWork unitOfWork,
        IPromotionRepository promotionRepository,
        IPromotionCnpjRepository promotionCnpjRepository,
        ICustomerRepository customerRepository) : this(unitOfWork)
    {
        _customerRepository = customerRepository;
        _promotionRepository = promotionRepository;
        _promotionCnpjRepository = promotionCnpjRepository;
    }

    protected async Task Commit()
    {
        await _unitOfWork.Commit();
    }

    protected async Task<(bool isValid, Notification notification)> ValidatePromotionsParameters(List<PromotionCnpj> promotionsCnpjs, string customerCnpj)
    {
        var notificationCode = "PromotionProduct";

        if (_customerRepository is null || _promotionRepository is null || _promotionCnpjRepository is null)
            return (false, new Notification(notificationCode, "Falha ao carregar repositórios."));

        var customer = await _customerRepository.FindBy(customer => customer.Cnpj == customerCnpj.OnlyNumbers());

        if (customer is null)
            return (false, new Notification(notificationCode, "Nenhum cliente encontrado para o CNPJ informado."));

        var promotions = await _promotionRepository.GetByAsync(promotionsCnpjs);
        var promotionsParameters = promotions.SelectMany(promotion => promotion.Parameters);

        var listAceptParameter = new List<string>();

        foreach (var promoParameter in promotionsParameters)
        {
            var item = customer.Parameters?.FirstOrDefault(p => promoParameter.Name?.ToLower() == p.Code?.ToLower() && promoParameter.Value?.ToLower() == p.Value?.ToLower());

            if (item is not null)
                listAceptParameter.Add(item.Code ?? "");
        }

        if (listAceptParameter.Distinct().Count() != promotionsParameters.Where(p => p.Name != null).Select(p => p.Name).Distinct().Count())
            return (false, new Notification(notificationCode, "Não foram encontrados `customers' com os parâmetros cadastrados na promoção."));

        return (true, new Notification(notificationCode, string.Empty));
    }
}
