namespace Acropolis.Application.Features.Promotions.Requests;

public class CreatePromotionAttributeRequest
{
    public CreatePromotionAttributeRequest()
    {
        SKUs = Array.Empty<string>();
    }

    public string AttributesId { get; set; } = null!;
    public IEnumerable<string> SKUs { get; set; }
    public decimal Qtd { get; set; }

    public static explicit operator PromotionAttribute(CreatePromotionAttributeRequest promotionAttributeRequest)
    {
        var skus = promotionAttributeRequest.SKUs?.Select(attributeSKURequest => new PromotionAttributeSku(attributeSKURequest)).ToList();

        return new(promotionAttributeRequest.AttributesId, promotionAttributeRequest.Qtd, skus);
    }
}