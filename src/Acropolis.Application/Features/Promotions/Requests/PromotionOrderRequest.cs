namespace Acropolis.Application.Features.Promotions.Requests
{
    public class PromotionOrderRequest
    {
        public PromotionOrderRequest()
        {
            ProductItems = new List<ProductAmountRequest>();
        }

        public Guid SellerId { get; set; }
        public string CustomerCode { get; set; } = null!;
        public IEnumerable<ProductAmountRequest> ProductItems { get; set; }
    }
}