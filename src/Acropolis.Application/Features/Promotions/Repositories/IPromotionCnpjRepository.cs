using Acropolis.Application.Base.Pagination;
using Acropolis.Application.Features.Promotions.Responses;

namespace Acropolis.Application.Features.Promotions.Repositories;

public interface IPromotionCnpjRepository
{
    Task<List<PromotionCnpj>> GetPromotionsByCNPJAsync(string CNPJ);
    Task<IEnumerable<PromotionCnpj>> GetAllByExternalIdAsync(string externalId, Guid sellerId);
    Task<PromotionCnpj?> GetByAsync(string externalId, string cnpj, Guid sellerId);
    Task<IPagedList<PromotionCnpjResponse>> FindAsync(PromotionCnpjParameters parameters, Guid sellerId);
    void Remove(PromotionCnpj promotionCnpj);
    Task<List<PromotionCnpj>> GetPromotionCNPJByCNPJAsync(string CNPJ, Guid sellerId);
    Task<List<PromotionCnpj>> GetPromotionCNPJByCNPJAsync(string CNPJ, IEnumerable<Guid> sellersIds);

}