namespace Acropolis.Application.Features.Promotions.Requests;

public class RemoveDiscountLimitRequest
{
    public string Cnpj { get; private set; }

    public RemoveDiscountLimitRequest(string cnpj)
    {
        Cnpj = cnpj;
    }
}
