using Acropolis.Application.Features.Attributes.Requests;
using Ziggurat;

namespace Acropolis.Consumer.Features.Attribute.Messages;

public class AttributeCreatedMessage : CreateAttributeRequest, IMessage
{
    public AttributeCreatedMessage(string messageId, string messageGroup)
    {
        MessageId = messageId;
        MessageGroup = messageGroup;
    }

    public string MessageId { get; set; }
    public string MessageGroup { get; set; }
}