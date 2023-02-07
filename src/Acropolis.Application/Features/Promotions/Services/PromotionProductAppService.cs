using Acropolis.Application.Base.Extensions;
using Acropolis.Application.Base.Models;
using Acropolis.Application.Base.Notifications;
using Acropolis.Application.Base.Persistence;
using Acropolis.Application.Base.Services;
using Acropolis.Application.Features.Attributes.Repositories;
using Acropolis.Application.Features.Attributes.Requests;
using Acropolis.Application.Features.Customers.Repositories;
using Acropolis.Application.Features.Products;
using Acropolis.Application.Features.Products.Repositories;
using Acropolis.Application.Features.Promotions.Repositories;
using Acropolis.Application.Features.Promotions.Requests;
using Acropolis.Application.Features.Promotions.Responses;
using Acropolis.Application.Features.Promotions.Services.Contracts;

namespace Acropolis.Application.Features.Promotions.Services;

public class PromotionProductAppService : AppService, IPromotionProductAppService
{
    private readonly IPromotionRepository _promotionRepository;
    private readonly IProductRepository _productRepository;
    private readonly IAttributeRepository _attributeRepository;
    private readonly IPromotionCnpjRepository _promotionCnpjRepository;
    
    public PromotionProductAppService(IUnitOfWork unitOfWork,
        IPromotionRepository promotionRepository,
        IProductRepository productRepository,
        IAttributeRepository attributeRepository,
        IPromotionCnpjRepository promotionCnpjRepository,
        ICustomerRepository customerRepository)
        : base(unitOfWork, promotionRepository, promotionCnpjRepository, customerRepository)
    {
        _promotionRepository = promotionRepository;
        _promotionCnpjRepository = promotionCnpjRepository;
        _productRepository = productRepository;
        _attributeRepository = attributeRepository;
    }

    public async Task<(Response, PromotionProductIncentiveListResponse?)> GetProductIncentiveListAsync(GetPromotionProductIncentiveListRequest request, CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(request.ProductId, out Guid productId))
            return (Response.Invalid(new Notification("PromotionProduct", "Identificador produto inválido.")), null);

        if (!Guid.TryParse(request.SellerId, out Guid sellerId))
            return (Response.Invalid(new Notification("PromotionProduct", "Identificador seller inválido.")), null);

        var product = await _productRepository.FindByAsync(product => product.Id == productId && product.SellerId == sellerId);

        if (product is null)
            return (Response.Invalid(new Notification("PromotionProduct", "Produto não encontrado.")), null);

        var customerCnpj = request.Cnpj.OnlyNumbers();
        var promotionsCnpjs = await _promotionCnpjRepository.GetPromotionCNPJByCNPJAsync(customerCnpj, sellerId);
        var (isValidPromotionsParameters, notification) = await ValidatePromotionsParameters(promotionsCnpjs, customerCnpj);
                
        if(!isValidPromotionsParameters)
            return (Response.Invalid(notification), null);

        var promotionsIds = promotionsCnpjs.Select(promotionCnpj => promotionCnpj.PromotionId);

        var (incentiveList, hasDiscount) = await BuildIncentiveListAsync(product, promotionsIds, request.TotalProductsLimit);

        return (Response.Valid(), new PromotionProductIncentiveListResponse(product, hasDiscount, incentiveList));
    }

    private async Task<(IEnumerable<Guid> incentiveList, bool hasDiscount)> BuildIncentiveListAsync(Product product, IEnumerable<Guid> promotionsIds, int pageSize)
    {
        if (!promotionsIds.Any())
            return (Enumerable.Empty<Guid>(), false);

        var attributesValuesIds = (await _attributeRepository.GetAllAttributesValuesIdsByProductIdAsync(product.Id)).Select(productAttributeId => productAttributeId.ToString().ToLower());

        if (!attributesValuesIds.Any())
            return (Enumerable.Empty<Guid>(), false);

        var promotionsProductAttributes = await _promotionRepository.FindWithProductAttributesAsync(new PromotionProductParameters(product.SellerId, attributesValuesIds, promotionsIds, pageSize));
        var promotionsProductAttributesCrossSell = Enumerable.Empty<PromotionResponse>();

        if (!promotionsProductAttributes.Any())
            return (Enumerable.Empty<Guid>(), false);

        var totalProductAttributesRuleLimit = 1;
        var promotionsAttributesValuesIds = GetPromotionsAttributesValuesIds(promotionsProductAttributes).Where(attrValueId => attributesValuesIds.Contains(attrValueId.ToLower())).ToList();

        if (promotionsAttributesValuesIds.Count > totalProductAttributesRuleLimit)
            promotionsProductAttributesCrossSell = await _promotionRepository.FindWithProductAttributesAsync(new PromotionProductParameters(product.SellerId, attributesValuesIds, promotionsIds, pageSize, true));            
        
        if(promotionsProductAttributesCrossSell.Any())
            promotionsAttributesValuesIds = GetPromotionsAttributesValuesIds(promotionsProductAttributesCrossSell).Where(attrValueId => !attributesValuesIds.Contains(attrValueId.ToLower())).ToList();

        var promotionIncentiveProductsCodes = promotionsProductAttributes.SelectMany(promotion => promotion.Attributes ?? new List<PromotionAttributeResponse>())
                                            .Where(attr => promotionsAttributesValuesIds.Contains(attr.AttributesId.ToLower()))
                                            .SelectMany(promotionAttribute => promotionAttribute.SKUs).Distinct().ToList();

        var promotionIncentiveProductsIds = new List<Guid>();

        if (promotionIncentiveProductsCodes.Any())
            promotionIncentiveProductsIds = (await _productRepository.FindAllIdsByAsync(p => promotionIncentiveProductsCodes.Any(materialCode => materialCode == p.MaterialCode) && 
                                            p.SellerId == product.SellerId && p.Id != product.Id, pageSize)).ToList();

        if (promotionIncentiveProductsIds.Count < pageSize)
        {
            var productIds = await ComplementIncentiveListAsync(product, promotionIncentiveProductsIds, promotionsIds, (pageSize - promotionIncentiveProductsIds.Count));
            promotionIncentiveProductsIds.AddRange(productIds);
        }

        return (promotionIncentiveProductsIds, true);
    }

    private async Task<IEnumerable<Guid>> ComplementIncentiveListAsync(Product product, IEnumerable<Guid> promotionIncentiveProductsIds, IEnumerable<Guid> promotionsIds, int pageSize)
    {
        var promotionsProductAttributes = await _promotionRepository.FindWithProductAttributesAsync(new PromotionProductParameters(product.SellerId, Enumerable.Empty<string>(), promotionsIds, pageSize));
        var promotionsAttributesValuesIds = GetPromotionsAttributesValuesIds(promotionsProductAttributes);
        var ignoreProductsIds = promotionIncentiveProductsIds.Concat(new List<Guid>() { product.Id });
        var attributesParameters = new AttributeParameters(promotionsAttributesValuesIds.Select(attributeId => Guid.Parse(attributeId)), ignoreProductsIds, pageSize);
        
        return await _attributeRepository.FindProductsIdsAsync(attributesParameters);
    }

    private static IEnumerable<string> GetPromotionsAttributesValuesIds(IEnumerable<PromotionResponse> promotions)
    {
        return promotions.SelectMany(promotion => promotion.Attributes ?? new List<PromotionAttributeResponse>())
               .Select(promotion => promotion.AttributesId.ToLower())
               .Distinct();
    }
}