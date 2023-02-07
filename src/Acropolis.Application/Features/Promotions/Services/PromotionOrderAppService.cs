using Acropolis.Application.Base.Converters;
using Acropolis.Application.Base.Extensions;
using Acropolis.Application.Base.Models;
using Acropolis.Application.Base.Notifications;
using Acropolis.Application.Base.Persistence;
using Acropolis.Application.Base.Services;
using Acropolis.Application.Features.Attributes.Enums;
using Acropolis.Application.Features.Attributes.Repositories;
using Acropolis.Application.Features.Customers.Repositories;
using Acropolis.Application.Features.Products;
using Acropolis.Application.Features.Products.Repositories;
using Acropolis.Application.Features.Promotions.Repositories;
using Acropolis.Application.Features.Promotions.Requests;
using Acropolis.Application.Features.Promotions.Responses;
using Acropolis.Application.Features.Promotions.Services.Contracts;

namespace Acropolis.Application.Features.Promotions.Services;

public class PromotionOrderAppService : AppService, IPromotionDiscountAppService
{
    private readonly IPromotionRepository _promotionRepository;
    private readonly IPromotionCnpjRepository _promotionCnpjRepository;
    private readonly IPromotionCnpjDiscountLimitRepository _promotionCnpjDiscountLimitRepository;
    private readonly IProductRepository _productRepository;
    private readonly IAttributeRepository _attributeRepository;
    private readonly ICustomerRepository _customerRepository;

    public PromotionOrderAppService(IUnitOfWork unitOfWork,
        IPromotionRepository promotionRepository,
        IPromotionCnpjRepository promotionCnpjRepository,
        IPromotionCnpjDiscountLimitRepository promotionCnpjDiscountLimitRepository,
        IProductRepository productRepository,
        IAttributeRepository attributeRepository,
        ICustomerRepository customerRepository)
        : base(unitOfWork, promotionRepository, promotionCnpjRepository, customerRepository)
    {
        _promotionRepository = promotionRepository;
        _promotionCnpjRepository = promotionCnpjRepository;
        _promotionCnpjDiscountLimitRepository = promotionCnpjDiscountLimitRepository;
        _productRepository = productRepository;
        _attributeRepository = attributeRepository;
        _customerRepository = customerRepository;
    }

    private sealed record ProductAttributes(Product Product, IEnumerable<Attributes.Attribute> Attributes);
    private sealed record PromotionData(IEnumerable<PromotionResponse> Promotions, IEnumerable<ProductAttributes> ProductAttributes);
    
    private int _promotionAttributesAmount;
    private int _promotionAttributesAmountMax;
    private decimal _maxDiscount;
    private List<string> _listIncentiveUpCross = new();
    private List<string> _listIncentiveCross = new();
    private IEnumerable<PromotionRuleResponse> _promotionsRules = new List<PromotionRuleResponse>();

    public async Task<(Response, PromotionOrderResponse)> GetPromotionOrderAsync(PromotionOrderRequest request, CancellationToken cancellationToken = default)
    {
        var hasDiscount = false;
        var response = new PromotionOrderResponse(hasDiscount);
        var customer = await _customerRepository.GetByCustomerCodeAsync(request.CustomerCode, request.SellerId);

        if ((customer?.Cnpj is null) || (customer?.Parameters is null))
            return (Response.Invalid(new Notification("PromotionOrder", "Cliente inválido.")), response);

        var promotionCnpj = await _promotionCnpjRepository.GetPromotionCNPJByCNPJAsync(customer.Cnpj, request.SellerId);
        var listPromotion = await _promotionRepository.GetByAsync(promotionCnpj);

        if ((listPromotion == null) || (!listPromotion.Any()))
            return (Response.Invalid(new Notification("PromotionOrder", "Não foi encontrada nenhuma promoção vinculada ao CustomerCode: " + request.CustomerCode + ".")), response);

        var (isValidPromotionsParameters, notification) = await ValidatePromotionsParameters(promotionCnpj, customer.Cnpj);

        if (!isValidPromotionsParameters)
            return (Response.Invalid(new Notification("PromotionOrder", notification.Description)), response); 

        var cnpj = customer.Cnpj.OnlyNumbers();
        var sellerId = request.SellerId;
        var productItems = request.ProductItems;
        var promotionsData = await GetPromotionData(sellerId, productItems, cnpj);

        if (!promotionsData.Promotions.Any() || (productItems is null || !productItems.Any()))
            return (Response.Invalid(new Notification("PromotionOrder", "Promoções válidas não foram encontradas.")), response);

        var orderMessage = "";
        var discountReached = GetDiscountReached(promotionsData, productItems);
        var discountLimit = await GetDiscountLimit(sellerId, cnpj);
        var incentiveMessage = BuildIncentiveMessages(discountLimit, discountReached, _maxDiscount);

        if (discountLimit < discountReached)
            discountReached = discountLimit;

        return (Response.Valid(), new PromotionOrderResponse(request.SellerId, new PromotionOrderBarResponse()
        {
            Percentage = discountReached,
            BarPercentage = CalculateBarPercentage(discountReached, discountLimit),
            MaxDiscountPercentage = discountLimit,
            MaxQuantityCategories = _promotionAttributesAmountMax,
            QuantityCategories = _promotionAttributesAmount,
        }, _listIncentiveUpCross.Count > 0 ? _listIncentiveUpCross.ConvertAll(x => x.ToLower()) : _listIncentiveCross.ConvertAll(x => x.ToLower()), !hasDiscount, orderMessage, incentiveMessage));
    }

    private async Task<decimal> GetDiscountLimit(Guid sellerId, string cnpj)
    {
        var promotionMaxDiscount = _promotionsRules.Any() ? _promotionsRules.Max(promotionRule => promotionRule.Percentage) : decimal.Zero;
        var promotionsCnpjDiscountLimit = await _promotionCnpjDiscountLimitRepository.GetObjectsCnpjsWithDiscountLimitsAsync(new List<string>() { cnpj }, sellerId);

        if (promotionsCnpjDiscountLimit.Any())
            return promotionsCnpjDiscountLimit.Max(limit => limit.Percent);

        return promotionMaxDiscount;
    }

    private decimal GetDiscountReached(PromotionData promotionsData, IEnumerable<ProductAmountRequest> productItems)
    {
        var promotionsAttributesTotal = 0;
        var discountAttributesMin = 1;
        var acceptedAttributes = new List<string>();

        _listIncentiveUpCross = new();
        _listIncentiveCross = new();
        _promotionAttributesAmount = 0;

        var productsAttributes = promotionsData.ProductAttributes;
        var productsAttributesValuesIds = productsAttributes.SelectMany(product => product.Attributes).Select(attr => attr.AttributeValueId).Distinct();
        var products = productsAttributes.Select(productAttr => productAttr.Product);
        var promotions = promotionsData.Promotions;

        _promotionsRules = promotions.SelectMany(promotion => promotion.Rules ?? new List<PromotionRuleResponse>()).OrderBy(r => r.GreaterEqualValue);
        var promotionsAttributes = promotions.SelectMany(promotion => promotion.Attributes ?? new List<PromotionAttributeResponse>());

        var promotionsAttributesIds = promotionsAttributes.Where(attr => productsAttributesValuesIds.Contains(attr.AttributesId.ToGuid())).Select(attr => attr.AttributesId).Distinct();
        var productsWeight = CalculateProductsWeight(productItems, products, promotions.FirstOrDefault()?.UnitMeasurement);

        _maxDiscount = _promotionsRules.OrderByDescending(t => t.TotalAttributes).ThenByDescending(g => g.GreaterEqualValue).FirstOrDefault()?.Percentage ?? decimal.Zero;

        var defaultRule = GetDefaultPromotionRule(discountAttributesMin, productsWeight);
        var attributesAmount = GetAttributesAmount(productItems, productsAttributes, products);

        foreach (var attrId in promotionsAttributesIds)
        {
            var promotionAttributeId = attrId.ToGuid();
            var promotionAttribute = promotionsAttributes.FirstOrDefault(attr => attr.AttributesId == attrId);
            var productAttributeAmountWeight = attributesAmount.ContainsKey(promotionAttributeId) ? attributesAmount[promotionAttributeId] : 0;

            if (promotionAttribute != null && productAttributeAmountWeight >= promotionAttribute.Amount)
            {
                promotionsAttributesTotal++;
                acceptedAttributes.Add(attrId);
            }
        }

        _promotionAttributesAmount = promotionsAttributesTotal;

        foreach (var attrItem in promotionsAttributes)
            _listIncentiveUpCross.Add(attrItem.AttributesId);

        var crossSellRule = GetCrossSellPromotionRule(promotionsAttributesTotal, productsWeight);
        var nextRowRule = GetNextRowPromotionRule(promotionsAttributesTotal, crossSellRule);

        if (nextRowRule == null)
        {
            var listIncentiveCrossData = promotionsAttributes.Where(attr => !acceptedAttributes.Contains(attr.AttributesId)).Select(attr => attr.AttributesId).Distinct();
            
            foreach (var attrItem in listIncentiveCrossData)
                _listIncentiveCross.Add(attrItem);

            if (listIncentiveCrossData.Any())
                _listIncentiveUpCross = new();
        }

        if (promotionsAttributesTotal <= discountAttributesMin)
            return (defaultRule?.Percentage ?? decimal.Zero);

        return (crossSellRule?.Percentage ?? (defaultRule?.Percentage ?? decimal.Zero));
    }

    private static Dictionary<Guid, decimal> GetAttributesAmount(IEnumerable<ProductAmountRequest> productItems, IEnumerable<ProductAttributes> productsAttributes, IEnumerable<Product> products)
    {
        var attributesAmount = new Dictionary<Guid, decimal>();

        foreach (var productItem in productItems)
        {
            var productAttributes = productsAttributes.FirstOrDefault(p => p.Product.Id == productItem.ProductId);
            var product = products.FirstOrDefault(product => product.Id == productItem.ProductId);

            if (productAttributes is null || product is null)
                continue;

            var unitMeasurement = WeightType.Ton.GetDescription();
            var productItemWeight = CalculateProductsWeight(new List<ProductAmountRequest>(){ productItem }, products, unitMeasurement);

            productAttributes.Attributes.ToList().ForEach(attr =>
            {
                if (!attributesAmount.ContainsKey(attr.AttributeValueId))
                    attributesAmount.Add(attr.AttributeValueId, productItemWeight);
                else
                    attributesAmount[attr.AttributeValueId] += productItemWeight;
            });
        }

        return attributesAmount;
    }

    private async Task<PromotionData> GetPromotionData(Guid sellerId, IEnumerable<ProductAmountRequest> productItems, string cnpj)
    {
        var emptyPromotionData = new PromotionData(Enumerable.Empty<PromotionResponse>(), Enumerable.Empty<ProductAttributes>());
        var productIds = productItems.Select(item => item.ProductId).Distinct();
        var products = await _productRepository.FindAllByIdsAsync(productIds);

        if (!products.Any())
            return emptyPromotionData;

        var attributesByProduct = (await _attributeRepository.FindAllByAsync(attr => productIds.Contains(attr.ProductId))).GroupBy(attr => attr.ProductId);

        if (!attributesByProduct.Any())
            return emptyPromotionData;

        var materialCodes = products.Where(p => !string.IsNullOrWhiteSpace(p.MaterialCode)).Select(p => $"{p.MaterialCode}");
        var promotionsIds = (await _promotionCnpjRepository.GetPromotionsByCNPJAsync(cnpj)).Select(promotionCnpj => promotionCnpj.PromotionId);

        if (!promotionsIds.Any())
            return emptyPromotionData;

        var promotions = await _promotionRepository.FindBySKUsAsync(new PromotionProductParameters(materialCodes, promotionsIds, sellerId));

        if (!promotions.Any())
            return emptyPromotionData;

        var productsAttributes = new List<ProductAttributes>();

        foreach (var productAttrs in attributesByProduct)
        {
            var product = products.FirstOrDefault(product => product.Id == productAttrs.Key);

            if (product is null || productsAttributes.Any(productAttr => productAttr.Product.Id == product.Id))
                continue;

            productsAttributes.Add(new ProductAttributes(product, productAttrs.ToList()));
        }

        _promotionAttributesAmountMax = promotions.SelectMany(promotion => promotion.Attributes ?? new List<PromotionAttributeResponse>()).Select(promotionAttribute => promotionAttribute.AttributesId).Distinct().Count();

        return new PromotionData(promotions, productsAttributes);
    }

    private string BuildIncentiveMessages(decimal discountLimit, decimal discountReached, decimal maxDiscount)
    {
        if ((discountLimit < discountReached) || (discountReached >= maxDiscount))
        {
            _listIncentiveUpCross = new();
            _listIncentiveCross = new();
            return ("<strong>Parabéns!</strong> Você conseguiu o desconto máximo disponivel para este pedido!");
        }

        if (_listIncentiveUpCross.Count > 0)
            return ("<strong>Aumente a quantidade ou adicione mais produtos</strong> para garantir desconto no seu pedido.");

        if (_listIncentiveCross.Count > 0)
            return ("<strong>Adicione mais produtos</strong> para garantir desconto máximo neste pedido!");

        return (string.Empty);
    }

    private PromotionRuleResponse? GetDefaultPromotionRule(int discountAttributesMin, decimal productsWeight) =>
        _promotionsRules.LastOrDefault(rule => rule.TotalAttributes <= discountAttributesMin && productsWeight >= rule.GreaterEqualValue);

    private PromotionRuleResponse? GetCrossSellPromotionRule(int promotionsAttributesTotal, decimal productsWeight) =>
        _promotionsRules.LastOrDefault(rule => rule.TotalAttributes == promotionsAttributesTotal && productsWeight >= rule.GreaterEqualValue);

    private PromotionRuleResponse? GetNextRowPromotionRule(int promotionsAttributesTotal, PromotionRuleResponse? crossSellRule) =>
        _promotionsRules.LastOrDefault(rule => rule.TotalAttributes == promotionsAttributesTotal && rule.GreaterEqualValue > crossSellRule?.GreaterEqualValue);

    private static decimal CalculateProductsWeight(IEnumerable<ProductAmountRequest> productsAmount, IEnumerable<Product> products, string? unitMeasurement)
    {
        var kilogram = WeightType.Kilogram.GetDescription();
        var ton = WeightType.Ton.GetDescription();
        var productsWeight = decimal.Zero;

        unitMeasurement = $"{unitMeasurement}".ToLower();

        foreach (var productAmount in productsAmount)
        {
            var product = products.FirstOrDefault(p => p.Id == productAmount.ProductId);

            if (product is null)
                continue;

            var convertedWeightProduct = decimal.Zero;

            if (unitMeasurement == ton)
                convertedWeightProduct = ($"{product.UnitMeasure}".Trim().ToLower() == kilogram) ? WeightConverter.ToTon(product.Weight) : product.Weight;
            else if (unitMeasurement == kilogram)
                convertedWeightProduct = ($"{product.UnitMeasure}".Trim().ToLower() == ton) ? WeightConverter.ToKilo(product.Weight) : product.Weight;

            if (convertedWeightProduct == decimal.Zero)
                continue;

            productsWeight += Math.Round(convertedWeightProduct * productAmount.Amount, 4);
        }

        return productsWeight;
    }

    private static decimal CalculateBarPercentage(decimal discountReached, decimal discountLimit)
    {
        if (discountReached == decimal.Zero || discountLimit == decimal.Zero)
            return decimal.Zero;

        return Math.Round(discountReached * 100 / discountLimit, 2);
    }
}