using Acropolis.Application.Features.Customers.Requests;
using Ziggurat;

namespace Acropolis.Consumer.Features.Customer.Messages;

public class CustomerRemovedMessage : RemoveCustomerRequest, IMessage
{
    public CustomerRemovedMessage(string messageId, string messageGroup)
    {
        MessageId = messageId;
        MessageGroup = messageGroup;
    }

    public string MessageId { get; set; }
    public string MessageGroup { get; set; }
}