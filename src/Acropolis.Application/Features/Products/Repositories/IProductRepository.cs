using System.Linq.Expressions;

namespace Acropolis.Application.Features.Products.Repositories;

public interface IProductRepository
{
    Task AddAsync(Product product);
    void Update(Product product);
    Task<Product?> GetByIdAsync(Guid productId);
    Task<Product?> FindByAsync(Expression<Func<Product, bool>> predicate);
    Task<IEnumerable<Guid>> FindAllIdsByAsync(Expression<Func<Product, bool>> predicate, int pageSize);
    Task<List<Guid>> FindProductIdsAsync(List<Guid> ids, List<Guid> sellerIds);
    Task<IEnumerable<Product>> FindAllByIdsAsync(IEnumerable<Guid> productIds);
    void Remove(Product product);
}
