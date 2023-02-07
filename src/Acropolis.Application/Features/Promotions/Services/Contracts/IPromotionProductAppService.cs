using Acropolis.Application.Base.Models;
using Acropolis.Application.Features.Promotions.Requests;
using Acropolis.Application.Features.Promotions.Responses;

namespace Acropolis.Application.Features.Promotions.Services.Contracts;

public interface IPromotionProductAppService
{
    Task<(Response, PromotionProductIncentiveListResponse?)> GetProductIncentiveListAsync(GetPromotionProductIncentiveListRequest request, CancellationToken cancellationToken = default);
}
