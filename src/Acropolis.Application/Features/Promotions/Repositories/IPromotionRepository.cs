using Acropolis.Application.Base.Pagination;
using Acropolis.Application.Features.Promotions.Requests;
using Acropolis.Application.Features.Promotions.Responses;

namespace Acropolis.Application.Features.Promotions.Repositories;

public interface IPromotionRepository
{
    Task AddAsync(Promotion promotion);
    Task<Promotion?> GetByExternalIdAsync(string externalId, Guid sellerId);
    Task<bool> HasPromotionAsync(string externalId);
    Task<IPagedList<PromotionResponse>> FindAsync(PromotionParameters parameters, Guid sellerId);
    Task<IEnumerable<PromotionResponse>> FindWithProductAttributesAsync(PromotionProductParameters parameters);
    Task<IEnumerable<PromotionResponse>> FindBySKUsAsync(PromotionProductParameters parameters);
    void Remove(Promotion promotion);
    Task<List<Promotion>> GetByAsync(List<PromotionCnpj> promotionCnpjs);
}
