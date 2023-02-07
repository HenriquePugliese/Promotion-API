using Acropolis.Application.Features.Products.Repositories;
using Acropolis.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Product = Acropolis.Application.Features.Products.Product;

namespace Acropolis.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly DbSet<Product> _products;

    public ProductRepository(AcropolisContext context)
    {
        _products = context.Products;
    }

    public async Task AddAsync(Product product)
    {
        await _products.AddAsync(product);
    }

    public void Update(Product product)
    {
        _products.Update(product);
    }

    public async Task<Product?> GetByIdAsync(Guid productId) =>
        await _products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == productId);

    public async Task<IEnumerable<Guid>> FindAllIdsByAsync(Expression<Func<Product, bool>> predicate, int pageSize) =>
        await _products.AsNoTracking().Where(predicate).Select(product => product.Id).Take(pageSize).ToListAsync();

    public async Task<Product?> FindByAsync(Expression<Func<Product, bool>> predicate) =>
        await _products.AsNoTracking().FirstOrDefaultAsync(predicate);

    public async Task<List<Guid>> FindProductIdsAsync(List<Guid> ids, List<Guid> sellerIds) =>
        await _products.AsNoTracking().Where(p => sellerIds.Contains(p.SellerId) && p.MaterialCode != null && ids.Contains(p.Id)).Select(p => p.Id).ToListAsync();

    public async Task<IEnumerable<Product>> FindAllByIdsAsync(IEnumerable<Guid> productIds) =>
        await _products.AsNoTracking().Where(product => productIds.Contains(product.Id)).ToListAsync();

    public void Remove(Product product)
    {
        _products.Remove(product);
    }
}
