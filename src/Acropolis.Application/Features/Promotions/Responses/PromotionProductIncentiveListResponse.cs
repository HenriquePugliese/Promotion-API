using Acropolis.Application.Base.Extensions;
using Acropolis.Application.Features.Products;

namespace Acropolis.Application.Features.Promotions.Responses
{
    public class PromotionProductIncentiveListResponse
    {
        public PromotionProductIncentiveListResponse(Product product, bool hasDiscount, IEnumerable<Guid>? incentiveList = null)
        {
            var materialCode = product.MaterialCode?.OnlyNumbers();

            if (!string.IsNullOrWhiteSpace(materialCode))
                MaterialCode = int.Parse(materialCode);

            SellerId = product.SellerId.ToString();
            ProductId = product.Id.ToString();
            HasDiscount = hasDiscount;
            IncentiveList = incentiveList ?? Enumerable.Empty<Guid>();
            PromotionMessage = hasDiscount ? "Produto da campanha D+" : string.Empty;
        }

        public string SellerId { get; private set; }
        public string ProductId { get; private set; }
        public string PromotionMessage { get; private set; }
        public int MaterialCode { get; private set; }
        public bool HasDiscount { get; private set; }
        public IEnumerable<Guid> IncentiveList { get; private set; }

    }
}
