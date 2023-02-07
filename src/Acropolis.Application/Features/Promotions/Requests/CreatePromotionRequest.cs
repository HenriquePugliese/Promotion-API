using Acropolis.Application.Features.Promotions.Enums;

namespace Acropolis.Application.Features.Promotions.Requests;

public class CreatePromotionRequest
{
    public CreatePromotionRequest()
    {
        Rules = Array.Empty<CreatePromotionRuleRequest>();
        Attributes = Array.Empty<CreatePromotionAttributeRequest>();
        Parameters = Array.Empty<CreatePromotionParameterRequest>();
    }

    public string Name { get; set; } = null!;
    public string ExternalId { get; set; } = null!;
    public string UnitMeasurement { get; set; } = null!;
    public DateTime DtStart { get; set; }
    public DateTime DtEnd { get; set; }
    public IEnumerable<CreatePromotionRuleRequest> Rules { get; set; }
    public IEnumerable<CreatePromotionAttributeRequest> Attributes { get; set; }
    public IEnumerable<CreatePromotionParameterRequest> Parameters { get; set; }

    public static explicit operator List<PromotionParameter>(CreatePromotionRequest promotionRequest)
    {
        var promotionParameterEnumNames = Enum.GetNames(typeof(Enums.PromotionParameter));
        var promotionParameters = new List<PromotionParameter>();

        promotionRequest.Parameters?.ToList().ForEach(promotionParameterRequest =>
        {
            var promotionParameterRequestType = promotionParameterRequest.GetType();

            foreach (var promotionParameterEnumName in promotionParameterEnumNames)
            {
                var parameterValues = promotionParameterRequestType?.GetProperty(promotionParameterEnumName)?.GetValue(promotionParameterRequest, null);

                if (parameterValues != null)
                    ((IEnumerable<string>)parameterValues).ToList().ForEach(parameterValue => promotionParameters.Add(new PromotionParameter(Enum.Parse<Enums.PromotionParameter>(promotionParameterEnumName), $"{parameterValue}")));
            }
        });

        return promotionParameters;
    }
}