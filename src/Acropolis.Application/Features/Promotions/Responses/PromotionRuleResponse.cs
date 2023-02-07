namespace Acropolis.Application.Features.Promotions.Responses;
public class PromotionRuleResponse
{
    public PromotionRuleResponse(int totalAttributes, decimal greaterEqualValue, decimal percentage)
    {
        TotalAttributes = totalAttributes;
        GreaterEqualValue = greaterEqualValue;
        Percentage = percentage;
    }

    public int TotalAttributes { get; private set; }
    public decimal GreaterEqualValue { get; private set; }
    public decimal Percentage { get; private set; }
}
