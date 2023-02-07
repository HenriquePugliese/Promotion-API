using System.Text.Json.Serialization;

namespace Acropolis.Application.Features.Promotions.Responses;
public class PromotionCnpjResponse
{
    public PromotionCnpjResponse(string externalId, IEnumerable<string>? cnpjs)
    {
        ExternalId = externalId;
        Cnpjs = cnpjs ?? Array.Empty<string>();
    }

    public PromotionCnpjResponse()
    {
    }

    public string ExternalId { get; private set; } = null!;

    [JsonPropertyName("CNPJs")]
    public IEnumerable<string> Cnpjs { get; private set; } = null!;
}