using Acropolis.Application.Base.Models;
using Acropolis.Application.Features.Promotions.Requests;
using Acropolis.Application.Features.Promotions.Responses;

namespace Acropolis.Application.Features.Promotions.Services.Contracts;

public interface IPromotionDiscountAppService
{
    Task<(Response, PromotionOrderResponse)> GetPromotionOrderAsync(PromotionOrderRequest request, CancellationToken cancellationToken = default);
}
