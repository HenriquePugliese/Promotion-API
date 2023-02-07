namespace Acropolis.Application.Features.Promotions.Requests;

public class RemovePromotionRequest
{
    public string ExternalId { get; set; }

    public RemovePromotionRequest(string externalId)
    {
        ExternalId = externalId;
    }
}
