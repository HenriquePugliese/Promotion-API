using Acropolis.Application.Features.Promotions.Requests;

namespace Acropolis.Tests.Helpers.Api;

public static class PromotionRequestHelper
{
    public static CreatePromotionRequest CreatePromotionRequestWithoutAttributes() =>
        new CreatePromotionRequest()
        {
            Name = "Promotion invalid",
            ExternalId = $"{Guid.NewGuid()}-uadj",
            DtStart = DateTime.Today,
            DtEnd = DateTime.Today.AddDays(10),
            UnitMeasurement = "kg",
            Rules = new List<CreatePromotionRuleRequest>() { new CreatePromotionRuleRequest() { Percentage = 1, TotalAttributes = 1, GreaterEqualValue = 1 } }
        };

    public static CreatePromotionRequest CreatePromotionRequestWithoutRules() =>
        new()
        {
            Name = "Promotion invalid",
            ExternalId = $"{Guid.NewGuid()}-aaa",
            DtStart = DateTime.Today,
            DtEnd = DateTime.Today.AddDays(10),
            UnitMeasurement = "kg",
            Attributes = new List<CreatePromotionAttributeRequest>() { new CreatePromotionAttributeRequest() { AttributesId = "1", Qtd = 1 } },
        };

    public static CreatePromotionRequest CreatePromotionRequestWithDateEndInvalid() =>
        new()
        {
            Name = "Promotion invalid",
            ExternalId = $"{Guid.NewGuid()}-ext-f-c",
            DtStart = DateTime.Today,
            DtEnd = DateTime.Today.AddDays(-1),
            UnitMeasurement = "kg",
            Attributes = new List<CreatePromotionAttributeRequest>() { new CreatePromotionAttributeRequest() { AttributesId = "1", Qtd = 1 } },
            Rules = new List<CreatePromotionRuleRequest>() { new CreatePromotionRuleRequest() { Percentage = 1, TotalAttributes = 1, GreaterEqualValue = 1 } }
        };

    public static CreatePromotionRequest CreatePromotionRequestWithPercentageInvalid(string? externalId = null) => new()
    {
        Name = "Promotion valid",
        ExternalId = externalId ?? $"{Guid.NewGuid()}-ext-ok-",
        DtStart = DateTime.Today,
        DtEnd = DateTime.Today.AddDays(10),
        UnitMeasurement = "kg",
        Attributes = new List<CreatePromotionAttributeRequest>() { new CreatePromotionAttributeRequest() { AttributesId = "1", Qtd = 1, SKUs = new List<string>() { "sku1" } } },
        Rules = new List<CreatePromotionRuleRequest>() { new CreatePromotionRuleRequest() { Percentage = 101, TotalAttributes = 1, GreaterEqualValue = 1 } },
        Parameters = new List<CreatePromotionParameterRequest>() { new CreatePromotionParameterRequest() { UF = new List<string>() { "SC" } } }
    };

    public static CreatePromotionRequest CreateValidPromotionRequest(string? externalId = null, string attributesId = "c7fbc3fd-34d6-4573-a089-aa5ce26B65b5") => new()
    {
        Name = "Promotion valid",
        ExternalId = externalId ?? $"{Guid.NewGuid()}-ext-ok-",
        DtStart = DateTime.Today,
        DtEnd = DateTime.Today.AddDays(10),
        UnitMeasurement = "kg",
        Attributes = new List<CreatePromotionAttributeRequest>() { new CreatePromotionAttributeRequest() { AttributesId = attributesId, Qtd = 1, SKUs = new List<string>() { "sku1" } } },
        Rules = new List<CreatePromotionRuleRequest>() { new CreatePromotionRuleRequest() { Percentage = 1, TotalAttributes = 1, GreaterEqualValue = 1 } },
        Parameters = new List<CreatePromotionParameterRequest>() { new CreatePromotionParameterRequest() { UF = new List<string>() { "SC" } } }
    };

    public static CreatePromotionRequest CreateInvalidPromotionRequest(string attributesId = "c7fbc3fd-34d6-4573-a089-aa5ce26B65b5") => new()
    {
        Name = "Promotion test invalid",
        ExternalId = string.Empty,
        DtStart = DateTime.Today,
        DtEnd = DateTime.Today.AddDays(5),
        UnitMeasurement = "kg",
        Attributes = new List<CreatePromotionAttributeRequest>() { new CreatePromotionAttributeRequest() { AttributesId = attributesId, Qtd = 1 } },
        Rules = new List<CreatePromotionRuleRequest>() { new CreatePromotionRuleRequest() { Percentage = 1, TotalAttributes = 1, GreaterEqualValue = 1 } }
    };
}