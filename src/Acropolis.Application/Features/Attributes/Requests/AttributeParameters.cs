using Acropolis.Application.Base.Models;

namespace Acropolis.Application.Features.Attributes.Requests;

public class AttributeParameters : Parameter
{
    public AttributeParameters(IEnumerable<Guid> attributesValuesIds, IEnumerable<Guid> ignoreProductsIds, int pageSize)
    {
        AttributesValuesIds = attributesValuesIds;
        IgnoreProductsIds = ignoreProductsIds;
        PageSize = pageSize;
    }

    public IEnumerable<Guid> IgnoreProductsIds { get; private set; }
    public IEnumerable<Guid> AttributesValuesIds { get; private set; }    
}