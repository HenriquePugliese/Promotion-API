using Acropolis.Application.Base.Models;
using Acropolis.Application.Features.Promotions.Requests;
using Acropolis.Application.Features.Promotions.Responses;

namespace Acropolis.Application.Features.Promotions.Services.Contracts;

public interface IPromotionAppService
{
    Task<(Response, Promotion?)> CreatePromotionAsync(CreatePromotionRequest request, Guid sellerId, CancellationToken cancellationToken = default);
    Task<Response> RemovePromotionAsync(RemovePromotionRequest request, Guid sellerId, CancellationToken cancellationToken = default);
    Task<(Response, PromotionResponse?)> GetPromotionAsync(string externalId, Guid sellerId);
    Task<(Response, PromotionCnpjResponse?)> AddCnpjsAsync(CreatePromotionCnpjRequest request, Guid sellerId, CancellationToken cancellationToken = default);
    Task<Response> RemoveCnpjAsync(RemovePromotionCnpjRequest request, Guid sellerId, CancellationToken cancellationToken = default);
}
