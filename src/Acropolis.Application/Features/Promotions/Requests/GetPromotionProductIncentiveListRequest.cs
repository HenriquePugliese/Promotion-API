using Acropolis.Application.Base.Extensions;

namespace Acropolis.Application.Features.Promotions.Requests
{
    public class GetPromotionProductIncentiveListRequest
    {
        public GetPromotionProductIncentiveListRequest(string productId, string sellerId, string cnpj, int totalProductsLimit = 10)
        {
            ProductId = productId;
            SellerId = sellerId;
            Cnpj = cnpj.OnlyNumbers();
            TotalProductsLimit = totalProductsLimit;
        }

        public string ProductId { get; private set; }
        public string SellerId { get; private set; }
        public string Cnpj { get; private set; }
        public int TotalProductsLimit { get; private set; }
    }
}