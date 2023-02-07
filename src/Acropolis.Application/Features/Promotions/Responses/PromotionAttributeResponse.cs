namespace Acropolis.Application.Features.Promotions.Responses;
public class PromotionAttributeResponse
{
    public PromotionAttributeResponse(string attributesId, decimal amount, IEnumerable<string>? skus)
    {
        AttributesId = attributesId;
        Amount = amount;
        SKUs = skus ?? new List<string>();
    }

    public string AttributesId { get; private set; }
    public decimal Amount { get; private set; }
    public IEnumerable<string> SKUs { get; private set; }
}
