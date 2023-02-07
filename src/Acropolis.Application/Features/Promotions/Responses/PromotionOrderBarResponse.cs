namespace Acropolis.Application.Features.Promotions.Responses
{
    public class PromotionOrderBarResponse
    {
        public decimal MaxDiscountPercentage { get; set; }
        public decimal Percentage { get; set; }
        public decimal BarPercentage { get; set; }
        public int MaxQuantityCategories { get; set; }
        public int QuantityCategories { get; set; }
    }
}
