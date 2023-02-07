using Acropolis.Application.Features.Attributes.Enums;

namespace Acropolis.Application.Features.Attributes.Requests;

public class AttributeRequest
{
    public Guid ProductId { get; set; }
    public Guid AttributeKeyId { get; set; }
    public string? AttributeKeyDescription { get; set; }
    public bool AttributeKeyIsBeginOpen { get; set; }
    public string? AttributeKey { get; set; }
    public string? AttributeKeyLabel { get; set; }
    public int AttributeKeyStatus { get; set; }
    public FilterType AttributeKeyType { get; set; }
    public Guid AttributeValueId { get; set; }
    public string? AttributeValueLabel { get; set; }
    public int AttributeValueStatus { get; set; }
    public string? AttributeValue { get; set; }
}