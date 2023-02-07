using Acropolis.Application.Base.Models;

namespace Acropolis.Application.Features.Promotions.Requests;

public class PromotionProductParameters : Parameter
{
    public PromotionProductParameters(Guid sellerId, IEnumerable<string> attributesValuesIds, IEnumerable<Guid> promotionsIds, int pageSize, bool considerDifferentAttributesIdsIntoPromotion = false)
    {
        SellerId = sellerId;
        AttributesValuesIds = attributesValuesIds;
        MaterialCodes = Enumerable.Empty<string>();
        PromotionsIds = promotionsIds;
        ConsiderDifferentAttributesIdsIntoPromotion = considerDifferentAttributesIdsIntoPromotion;
        PageSize = pageSize;
    }

    public PromotionProductParameters(IEnumerable<string> materialCodes, IEnumerable<Guid> promotionsIds, Guid sellerId, int pageSize = 10) : this(sellerId, Enumerable.Empty<string>(), promotionsIds, pageSize, false)
    {
        MaterialCodes = materialCodes;
    }

    public Guid SellerId { get; private set; }

    public bool ConsiderDifferentAttributesIdsIntoPromotion { get; private set; }

    public IEnumerable<string> AttributesValuesIds { get; private set; }

    public IEnumerable<Guid> PromotionsIds { get; private set; }

    public IEnumerable<string> MaterialCodes { get; private set; }
}