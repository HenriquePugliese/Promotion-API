using Acropolis.Application.Base.Pagination;
using Acropolis.Application.Features.Promotions;
using Acropolis.Application.Features.Promotions.Repositories;
using Acropolis.Application.Features.Promotions.Responses;
using Acropolis.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Acropolis.Infrastructure.Repositories;

public class PromotionCnpjRepository : IPromotionCnpjRepository
{
    private readonly DbSet<PromotionCnpj> _promotionsCnpjs;

    public PromotionCnpjRepository(AcropolisContext context)
    {
        _promotionsCnpjs = context.PromotionsCnpjs;
    }

    public async Task<List<PromotionCnpj>> GetPromotionsByCNPJAsync(string CNPJ) =>
        await _promotionsCnpjs.AsNoTracking().Where(c => c.Cnpj == CNPJ).ToListAsync();

    public async Task<IEnumerable<PromotionCnpj>> GetAllByExternalIdAsync(string externalId, Guid sellerId)
    {
        return await _promotionsCnpjs.Where(promotion => promotion.ExternalId == externalId && promotion.SellerId == sellerId).ToListAsync();
    }

    public async Task<PromotionCnpj?> GetByAsync(string externalId, string cnpj, Guid sellerId)
    {
        return await _promotionsCnpjs.Where(promotion => promotion.ExternalId == externalId && promotion.Cnpj == cnpj && promotion.SellerId == sellerId).FirstOrDefaultAsync();
    }

    public void Remove(PromotionCnpj promotionCnpj)
    {
        _promotionsCnpjs.Remove(promotionCnpj);
    }

    public async Task<IPagedList<PromotionCnpjResponse>> FindAsync(PromotionCnpjParameters parameters, Guid sellerId)
    {
        var query = _promotionsCnpjs.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(parameters.ExternalId))
        {
            query = query.Where(promotionCnpj => promotionCnpj.ExternalId == parameters.ExternalId && promotionCnpj.SellerId == sellerId);
        }

        if (parameters.Cnpjs is not null && parameters.Cnpjs.Any())
        {
            query = query.Where(promotionCnpj => parameters.Cnpjs.Contains(promotionCnpj.Cnpj));
        }

        var total = await query.CountAsync();

        var cnpjs = await query
            .Skip((parameters.PageIndex - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .Select(promotion => promotion.Cnpj)
            .ToListAsync();

        var items = new List<PromotionCnpjResponse> { new PromotionCnpjResponse(parameters.ExternalId, cnpjs) };

        return new PagedList<PromotionCnpjResponse>(items, total, parameters.PageIndex, parameters.PageSize);
    }

    public async Task<List<PromotionCnpj>> GetPromotionCNPJByCNPJAsync(string CNPJ, Guid sellerId) => await _promotionsCnpjs.Where(c => c.Cnpj == CNPJ && c.SellerId == sellerId).ToListAsync();

    public async Task<List<PromotionCnpj>> GetPromotionCNPJByCNPJAsync(string CNPJ, IEnumerable<Guid> sellersIds) => await _promotionsCnpjs.Where(c => c.Cnpj == CNPJ && sellersIds.Contains(c.SellerId)).ToListAsync();

}
