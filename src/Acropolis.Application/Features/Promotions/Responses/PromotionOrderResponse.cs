namespace Acropolis.Application.Features.Promotions.Responses
{
    public class PromotionOrderResponse
    {
        public PromotionOrderResponse(bool hasDiscount)
        {
            HasDiscount = hasDiscount;
        }

        public PromotionOrderResponse(Guid sellerId, bool hasDiscount)
        {
            HasDiscount = hasDiscount;
            SellerId = sellerId;
        }

        public PromotionOrderResponse(Guid sellerId, PromotionOrderBarResponse orderBar, List<string> incentiveAttributes, bool hasDiscount)
            : this(sellerId, hasDiscount)
        {
            OrderBar = orderBar;
            IncentiveAttributes = incentiveAttributes;
        }

        public PromotionOrderResponse(Guid sellerId, PromotionOrderBarResponse orderBar, List<string> incentiveAttributes, bool hasDiscount, string orderMessage, string incentiveMessage) 
            : this(sellerId, orderBar, incentiveAttributes, hasDiscount)
        {
            OrderMessage = orderMessage;
            IncentiveMessge = incentiveMessage;
        }

        public Guid SellerId { get; private set; }
        public string? OrderMessage { get; private set; }
        public string? IncentiveMessge { get; private set; }
        public bool HasDiscount { get; private set; }
        public PromotionOrderBarResponse? OrderBar { get; private set; }
        public List<string> IncentiveAttributes { get; private set; } = new ();
    }
}