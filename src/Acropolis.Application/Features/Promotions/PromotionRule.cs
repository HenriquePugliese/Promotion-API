using Acropolis.Application.Base.Models;

namespace Acropolis.Application.Features.Promotions;

public class PromotionRule : Entity
{
    public PromotionRule(int totalAttributes, decimal value, decimal percentage, Promotion? promotion = null)
    {
        TotalAttributes = totalAttributes;
        GreaterEqualValue = value;
        Percentage = percentage;
        Promotion = promotion;
        PromotionId = promotion?.Id ?? Guid.Empty;
    }

    private PromotionRule()
    {
    }

    public int TotalAttributes { get; private set; }
    public decimal GreaterEqualValue { get; private set; }
    public decimal Percentage { get; private set; }
    public Guid PromotionId { get; set; }
    public virtual Promotion? Promotion { get; set; }
}