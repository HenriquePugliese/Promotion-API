namespace Acropolis.Application.Features.Promotions.Requests;

public class CreateDiscountLimitRequest
{
    public CreateDiscountLimitRequest()
    {
    }

    public string Cnpj { get; set; } = null!;
    public decimal Percent { get; set; }
}