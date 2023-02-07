using Acropolis.Application.Base.Pagination;
using Acropolis.Application.Features.Promotions;
using Acropolis.Application.Features.Promotions.Responses;
using Acropolis.Application.Features.Promotions.Repositories;
using Acropolis.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using Acropolis.Application.Features.Promotions.Requests;
using Acropolis.Application.Features.Promotions.Enums;

namespace Acropolis.Infrastructure.Repositories;

public class PromotionRepository : IPromotionRepository
{
    private readonly DbSet<Promotion> _promotions;

    public PromotionRepository(AcropolisContext context)
    {
        _promotions = context.Promotions;
    }

    public async Task AddAsync(Promotion promotion)
    {
        await _promotions.AddAsync(promotion);
    }

    public async Task<IPagedList<PromotionResponse>> FindAsync(PromotionParameters parameters, Guid sellerId)
    {
        var query = _promotions.AsNoTracking();

        if (parameters.Name is not null)
        {
            query = query.Where(p => p.Name != null && p.Name.StartsWith(parameters.Name) && p.SellerId == sellerId);
        }

        query = parameters.OrderBy switch
        {
            "name-asc" => query.OrderBy(u => u.Name),
            "name-desc" => query.OrderByDescending(u => u.Name),
            _ => query.OrderBy(u => u.Name),
        };

        var total = await query.CountAsync();

        var items = await query
            .Skip((parameters.PageIndex - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .Include(promotion => promotion.Parameters)
            .Include(promotion => promotion.Rules)
            .Include(promotion => promotion.Attributes)
            .ThenInclude(attribute => attribute.SKUs)
            .Select(promotion => new PromotionResponse(promotion))
            .ToListAsync();

        return new PagedList<PromotionResponse>(items, total, parameters.PageIndex, parameters.PageSize);
    }

    public async Task<Promotion?> GetByExternalIdAsync(string externalId, Guid sellerId)
    {
        return await _promotions
               .Include(promotion => promotion.Parameters)
               .Include(promotion => promotion.Rules)
               .Include(promotion => promotion.Attributes)
               .ThenInclude(attribute => attribute.SKUs)
               .FirstOrDefaultAsync(promotion => promotion.ExternalId == externalId && promotion.SellerId == sellerId);
    }

    public async Task<IEnumerable<PromotionResponse>> FindWithProductAttributesAsync(PromotionProductParameters parameters)
    {
        var query = _promotions.AsNoTracking();

        if (parameters.AttributesValuesIds is not null && parameters.AttributesValuesIds.Any())
            query = query.Where(promotion => promotion.Attributes != null &&
                    promotion.Attributes.Any(promotionAttribute => parameters.ConsiderDifferentAttributesIdsIntoPromotion ? !parameters.AttributesValuesIds.Contains(promotionAttribute.AttributesId) : parameters.AttributesValuesIds.Contains(promotionAttribute.AttributesId)));

        if (!parameters.SellerId.Equals(Guid.Empty))
            query = query.Where(promotion => promotion.SellerId == parameters.SellerId);

        if (parameters.PromotionsIds is not null && parameters.PromotionsIds.Any())
            query = query.Where(promotion => parameters.PromotionsIds.Contains(promotion.Id));

        query = query
           .Where(promotion => promotion.Status == PromotionStatus.Active)
           .Where(promotion => promotion.DtStart <= DateTime.Now)
           .Where(promotion => promotion.DtEnd > DateTime.Now);

        return await query
            .Include(promotion => promotion.Attributes)
            .ThenInclude(attribute => attribute.SKUs)
            .OrderBy(promotion => EF.Functions.Random())
            .Select(promotion => new PromotionResponse(promotion))
            .ToListAsync();
    }

    public async Task<IEnumerable<PromotionResponse>> FindBySKUsAsync(PromotionProductParameters parameters)
    {
        var query = _promotions.AsNoTracking();

        if (parameters.MaterialCodes is not null && parameters.MaterialCodes.Any())
            query = query.Where(promotion => promotion.Attributes != null &&
                    promotion.Attributes.Any(promotionAttribute => promotion.Attributes.Any(attr => attr.SKUs != null && 
                    attr.SKUs.Any(sku => parameters.MaterialCodes.Contains(sku.Value)))));

        if (!parameters.SellerId.Equals(Guid.Empty))
            query = query.Where(promotion => promotion.SellerId == parameters.SellerId);

        if (parameters.PromotionsIds is not null && parameters.PromotionsIds.Any())
            query = query.Where(promotion => parameters.PromotionsIds.Contains(promotion.Id));

        query = query.Where(promotion => promotion.Status == PromotionStatus.Active && promotion.DtStart <= DateTime.Now && promotion.DtEnd > DateTime.Now);

        return await query
            .Take(parameters.PageSize)
            .Include(promotion => promotion.Parameters)
            .Include(promotion => promotion.Rules)
            .Include(promotion => promotion.Attributes)
            .ThenInclude(attribute => attribute.SKUs)
            .Select(promotion => new PromotionResponse(promotion))
            .ToListAsync();
    }

    public async Task<bool> HasPromotionAsync(string externalId) => await _promotions.AnyAsync(promotion => promotion.ExternalId == externalId);

    public void Remove(Promotion promotion)
    {
        _promotions.Remove(promotion);
    }

    public async Task<List<Promotion>> GetByAsync(List<PromotionCnpj> promotionCnpjs)
    {
        var promotionsIds = promotionCnpjs.Select(promotionCnpj => promotionCnpj.PromotionId).Distinct();

        return await _promotions
                    .Where(p => promotionsIds.Contains(p.Id) && p.DtStart <= DateTime.Now && p.DtEnd >= DateTime.Now)
                   .Include(promotion => promotion.Parameters)
                   .Include(promotion => promotion.Rules)
                   .Include(promotion => promotion.Attributes)
                   .ThenInclude(attribute => attribute.SKUs)
                   .ToListAsync();
    }
}
