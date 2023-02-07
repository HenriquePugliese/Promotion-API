using Acropolis.Application.Base.Models;

namespace Acropolis.Application.Features.Promotions;

public class PromotionAttribute : Entity
{
    public PromotionAttribute(string attributesId, decimal amountWeight, List<PromotionAttributeSku>? skus, Promotion? promotion = null)
    {
        AttributesId = attributesId;
        AmountWeight = amountWeight;
        SKUs = skus ?? new List<PromotionAttributeSku>();
        Promotion = promotion;
        PromotionId = promotion?.Id ?? Guid.Empty;
    }
    
    private PromotionAttribute()
    {
    }

    public string? AttributesId { get; private set; }
    public decimal AmountWeight { get; private set; }
    public List<PromotionAttributeSku>? SKUs { get; private set; }
    public Guid PromotionId { get; set; }
    public virtual Promotion? Promotion { get; set; }
}