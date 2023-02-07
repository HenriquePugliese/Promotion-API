using Acropolis.Application.Features.Attributes.Repositories;
using Acropolis.Application.Features.Attributes.Requests;
using Acropolis.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Attribute = Acropolis.Application.Features.Attributes.Attribute;

namespace Acropolis.Infrastructure.Repositories;

public class AttributeRepository : IAttributeRepository
{
    private readonly DbSet<Attribute> _attributes;

    public AttributeRepository(AcropolisContext context)
    {
        _attributes = context.Attributes;
    }

    public async Task AddAsync(Attribute attribute)
    {
        await _attributes.AddAsync(attribute);
    }

    public void Update(Attribute attribute)
    {
        _attributes.Update(attribute);
    }

    public async Task<Attribute?> GetByIdAsync(Guid attributeId) =>
        await _attributes.AsNoTracking().FirstOrDefaultAsync(p => p.Id == attributeId);

    public async Task<IEnumerable<Guid>> GetAllAttributesValuesIdsByProductIdAsync(Guid productId) =>
        await _attributes.AsNoTracking().Where(attribute => attribute.ProductId == productId).Select(attribute => attribute.AttributeValueId).ToListAsync();

    public async Task<IEnumerable<Guid>> FindProductsIdsAsync(AttributeParameters parameters)
    {
        var query = _attributes.AsNoTracking();

        if (parameters.AttributesValuesIds.Any())
            query = query.Where(attribute => parameters.AttributesValuesIds.Contains(attribute.AttributeValueId));        

        if (parameters.IgnoreProductsIds.Any())        
            query = query.Where(attribute => !parameters.IgnoreProductsIds.Contains(attribute.ProductId));
        
        return await query
            .Take(parameters.PageSize)
            .OrderBy(promotion => EF.Functions.Random())
            .Select(attribute => attribute.ProductId)
            .Distinct()
            .ToListAsync();
    }

    public async Task<Attribute?> FindByAsync(Expression<Func<Attribute, bool>> predicate) => 
        await _attributes.AsNoTracking().FirstOrDefaultAsync(predicate);

    public async Task<IEnumerable<Attribute>> FindAllByAsync(Expression<Func<Attribute, bool>> predicate) =>
          await _attributes.AsNoTracking().Where(predicate).ToListAsync();    

    public void Remove(Attribute attribute)
    {
        _attributes.Remove(attribute);
    }

    
}

