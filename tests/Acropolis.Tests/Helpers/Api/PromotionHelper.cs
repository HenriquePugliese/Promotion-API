using Acropolis.Application.Features.Promotions;

namespace Acropolis.Tests.Helpers.Api;


public static class PromotionHelper
{
    public static Promotion CreateValidPromotion(Guid sellerId, string? externalId = null) => new(PromotionRequestHelper.CreateValidPromotionRequest(externalId),sellerId);
}
