namespace Acropolis.Application.Features.Promotions.Requests
{
    public class ProductAmountRequest
    {
        public Guid ProductId { get; set; }
        public decimal Amount { get; set; }
    }
}