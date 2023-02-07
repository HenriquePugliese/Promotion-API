using Acropolis.Application.Features.Attributes.Requests;
using System.Linq.Expressions;

namespace Acropolis.Application.Features.Attributes.Repositories;

public interface IAttributeRepository
{
    Task AddAsync(Attribute attribute);

    void Update(Attribute attribute);

    Task<Attribute?> GetByIdAsync(Guid attributeId);

    Task<IEnumerable<Guid>> GetAllAttributesValuesIdsByProductIdAsync(Guid productId);

    Task<Attribute?> FindByAsync(Expression<Func<Attribute, bool>> predicate);

    Task<IEnumerable<Attribute>> FindAllByAsync(Expression<Func<Attribute, bool>> predicate);

    Task<IEnumerable<Guid>> FindProductsIdsAsync(AttributeParameters parameters);

    void Remove(Attribute attribute);
}