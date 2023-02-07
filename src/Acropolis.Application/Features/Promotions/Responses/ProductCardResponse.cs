using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acropolis.Application.Features.Promotions.Responses;

public class ProductCardResponse
{
    public ProductCardResponse(string cnpj)
    {
        CNPJ = cnpj;
        Products = new List<ProductPromotionResponse>();
    }

    public string CNPJ { get; set; }
    public List<ProductPromotionResponse> Products { get; set; }
}

public class ProductPromotionResponse
{
    public ProductPromotionResponse(string productId, bool hasDiscount)
    {
        ProductId = productId;
        HasDiscount = hasDiscount;
        PromotionMessage = (hasDiscount) ? "Produto da campanha D+" : string.Empty;
    }

    public string ProductId { get; set; }
    public bool HasDiscount { get; set; }
    public string PromotionMessage { get; set; }
}