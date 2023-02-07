using Acropolis.Application.Features.Promotions.Requests;
using Acropolis.Application.Features.Promotions.Responses;

namespace Acropolis.Application.Features.Promotions.Services.Contracts;

public interface IProductCardAppService
{
    Task<ProductCardResponse> ProductListHasPromotionAsync(ProductCardRequest productCardRequest);
}
