namespace Acropolis.Application.Features.Promotions.Responses;
public class DiscountLimitResponse
{
    public DiscountLimitResponse(PromotionCnpjDiscountLimit discountLimit)
    {
        Cnpj = discountLimit.Cnpj;
        Percent = discountLimit.Percent;
    }

    private DiscountLimitResponse()
    {
    }

    public string Cnpj { get; private set; } = null!;
    public decimal Percent { get; private set; }
}