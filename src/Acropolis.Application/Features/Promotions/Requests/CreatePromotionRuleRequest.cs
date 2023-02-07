namespace Acropolis.Application.Features.Promotions.Requests;

public class CreatePromotionRuleRequest
{
    public int TotalAttributes { get; set; }
    public decimal Percentage { get; set; }
    public decimal GreaterEqualValue { get; set; }

    public static explicit operator PromotionRule(CreatePromotionRuleRequest promotionRuleRequest) => 
        new(promotionRuleRequest.TotalAttributes, promotionRuleRequest.GreaterEqualValue, promotionRuleRequest.Percentage);
}