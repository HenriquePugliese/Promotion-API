using Acropolis.Application.Features.Attributes.Enums;
using Acropolis.Application.Features.Attributes.Requests;

namespace Acropolis.Application.Features.Attributes;

public class Attribute
{
    public Attribute(CreateAttributeRequest attributeRequest, Guid? id = null)
    {
        ProductId = attributeRequest.ProductId;
        AttributeKeyId = attributeRequest.AttributeKeyId;
        AttributeKeyDescription = attributeRequest.AttributeKeyDescription;
        AttributeKeyIsBeginOpen = attributeRequest.AttributeKeyIsBeginOpen;
        AttributeKey = attributeRequest.AttributeKey;
        AttributeKeyLabel = attributeRequest.AttributeKeyLabel;
        AttributeKeyStatus = attributeRequest.AttributeKeyStatus;
        AttributeKeyType = attributeRequest.AttributeKeyType;
        AttributeValueId = attributeRequest.AttributeValueId;
        AttributeValueLabel = attributeRequest.AttributeValueLabel;
        AttributeValueStatus = attributeRequest.AttributeValueStatus;
        AttributeValue = attributeRequest.AttributeValue;
        Id = id ?? Guid.NewGuid();
    }

    private Attribute()
    {
    }

    public Guid Id { get; private set; }
    public Guid ProductId { get; private set; }
    public Guid AttributeKeyId { get; private set; }
    public string? AttributeKeyDescription { get; private set; }
    public bool AttributeKeyIsBeginOpen { get; private set; }
    public string? AttributeKey { get; private set; }
    public string? AttributeKeyLabel { get; private set; }
    public int AttributeKeyStatus { get; private set; }
    public FilterType AttributeKeyType { get; private set; }
    public Guid AttributeValueId { get; private set; }
    public string? AttributeValueLabel { get; private set; }
    public int AttributeValueStatus { get; private set; }
    public string? AttributeValue { get; private set; }
}