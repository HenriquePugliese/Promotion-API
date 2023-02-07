namespace Acropolis.Application.Features.Promotions.Repositories;

public interface IPromotionCnpjDiscountLimitRepository
{
    Task AddAsync(PromotionCnpjDiscountLimit discountLimit);
    void Update(PromotionCnpjDiscountLimit discountLimit);
    Task<PromotionCnpjDiscountLimit?> GetByCnpjAsync(string cnpj, Guid sellerId);
    Task<IEnumerable<string>> GetCnpjsWithDiscountLimitsAsync(IEnumerable<string> cnpjs, Guid sellerId);
    Task<IEnumerable<PromotionCnpjDiscountLimit>> GetObjectsCnpjsWithDiscountLimitsAsync(IEnumerable<string> cnpjs, Guid sellerId);
    void Remove(PromotionCnpjDiscountLimit discountLimit);
}