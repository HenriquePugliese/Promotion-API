using Acropolis.Application.Base.Models;

namespace Acropolis.Application.Features.Promotions;

public class PromotionAttributeSku : Entity
{
    public PromotionAttributeSku(string value, PromotionAttribute? promotionAttribute = null)
    {
        Value = value;
        PromotionAttribute = promotionAttribute;
        PromotionAttributeId = promotionAttribute?.Id ?? Guid.Empty;
    }
    
    private PromotionAttributeSku()
    {
    }

    public string Value { get; private set; } = null!;    
    public Guid PromotionAttributeId { get; set; }
    public virtual PromotionAttribute? PromotionAttribute { get; set; }
}