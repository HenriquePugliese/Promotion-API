using Acropolis.Application.Features.Promotions.Enums;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Acropolis.Application.Features.Promotions.Responses;
public class PromotionResponse
{
    public string Name { get; set; } = null!;
    public string ExternalId { get; set; } = null!;
    public string UnitMeasurement { get; private set; } = null!;
    public DateTime DtStart { get; set; }
    public DateTime DtEnd { get; set; }
    public PromotionStatus Status { get; set; }
    public string? StatusName => Status.GetType()
        .GetMember(Status.ToString())
        .FirstOrDefault()?
        .GetCustomAttribute<DisplayAttribute>()?
        .GetName();
    public List<PromotionParameterResponse>? Parameters { get; set; }
    public List<PromotionAttributeResponse>? Attributes { get; set; }
    public List<PromotionRuleResponse>? Rules { get; set; }

    public PromotionResponse()
    {
    }

    public PromotionResponse(Promotion promotion)
    {
        Name = promotion.Name;
        ExternalId = promotion.ExternalId;
        UnitMeasurement = promotion.UnitMeasurement;
        DtStart = promotion.DtStart;
        DtEnd = promotion.DtEnd;
        Status = promotion.Status;
        Parameters = promotion.Parameters?.Select(parameter => new PromotionParameterResponse($"{parameter.Name}", $"{parameter.Value}")).ToList();
        Attributes = promotion.Attributes?.Select(attribute => new PromotionAttributeResponse($"{attribute.AttributesId}", attribute.AmountWeight, attribute.SKUs?.Select(sku => sku.Value))).ToList();
        Rules = promotion.Rules?.Select(rule => new PromotionRuleResponse(rule.TotalAttributes, rule.GreaterEqualValue, rule.Percentage)).ToList();
    }

}

