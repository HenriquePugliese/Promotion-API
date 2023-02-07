using Acropolis.Application.Base.Models;
using Acropolis.Application.Features.Promotions.Requests;
using Acropolis.Application.Features.Promotions.Responses;

namespace Acropolis.Application.Features.Promotions.Services.Contracts;

public interface IDiscountLimitAppService
{
    Task<(Response, IEnumerable<PromotionCnpjDiscountLimit>?)> CreateDiscountLimitAsync(IEnumerable<CreateDiscountLimitRequest> request, Guid sellerId, CancellationToken cancellationToken = default);
    Task<Response> RemoveDiscountLimitAsync(RemoveDiscountLimitRequest request, Guid sellerId, CancellationToken cancellationToken = default);
    Task<(Response, DiscountLimitResponse?)> GetDiscountLimitAsync(string cnpj, Guid sellerId);
}