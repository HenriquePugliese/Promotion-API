using Acropolis.Application.Features.Promotions;
using Acropolis.Application.Features.Promotions.Repositories;
using Acropolis.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Acropolis.Infrastructure.Repositories;

public class PromotionCnpjDiscountLimitRepository : IPromotionCnpjDiscountLimitRepository
{
    private readonly DbSet<PromotionCnpjDiscountLimit> _promotionsCnpjsDiscountLimits;

    public PromotionCnpjDiscountLimitRepository(AcropolisContext context)
    {
        _promotionsCnpjsDiscountLimits = context.PromotionsCnpjsDiscountLimits;
    }

    public async Task AddAsync(PromotionCnpjDiscountLimit discountLimit)
    {
        await _promotionsCnpjsDiscountLimits.AddRangeAsync(discountLimit);
    }

    public void Update(PromotionCnpjDiscountLimit discountLimit)
    {
        _promotionsCnpjsDiscountLimits.Update(discountLimit);
    }

    public async Task<PromotionCnpjDiscountLimit?> GetByCnpjAsync(string cnpj, Guid sellerId)
    {
        return await _promotionsCnpjsDiscountLimits
               .FirstOrDefaultAsync(discountLimit => discountLimit.Cnpj == cnpj && discountLimit.SellerId == sellerId);
    }

    public async Task<IEnumerable<string>> GetCnpjsWithDiscountLimitsAsync(IEnumerable<string> cnpjs, Guid sellerId) {
        return await _promotionsCnpjsDiscountLimits
               .Where(discountLimit => cnpjs.Contains(discountLimit.Cnpj) && discountLimit.SellerId == sellerId)
               .Select(discountLimit => discountLimit.Cnpj)
               .ToListAsync();
    }

    public async Task<IEnumerable<PromotionCnpjDiscountLimit>> GetObjectsCnpjsWithDiscountLimitsAsync(IEnumerable<string> cnpjs, Guid sellerId)
    {
        return await _promotionsCnpjsDiscountLimits
               .Where(discountLimit => cnpjs.Contains(discountLimit.Cnpj) && discountLimit.SellerId == sellerId)
               .OrderBy(discountLimit => discountLimit.Percent)
               .ToListAsync();
    }

    public void Remove(PromotionCnpjDiscountLimit discountLimit)
    {
        _promotionsCnpjsDiscountLimits.Remove(discountLimit);
    }
}