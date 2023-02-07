using Acropolis.Application.Base.Extensions;

namespace Acropolis.Application.Features.Promotions.Requests;
public class ProductCardRequest
{
    public ProductCardRequest(string cnpj)
    {
        CNPJ = cnpj.OnlyNumbers();
        ProductsList = new List<ProductSellerRequest>();
    }

    public List<ProductSellerRequest> ProductsList { get; set; }
    public string CNPJ { get; set; }
}


