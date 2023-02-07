using Acropolis.Application.Features.Products.Requests;
using Ziggurat;

namespace Acropolis.Consumer.Features.Product.Messages;

public class ProductChangedMessage : UpdateProductRequest, IMessage
{
    public ProductChangedMessage(string messageId, string messageGroup)
    {
        MessageId = messageId;
        MessageGroup = messageGroup;
    }

    public string MessageId { get; set; }
    public string MessageGroup { get; set; }
}