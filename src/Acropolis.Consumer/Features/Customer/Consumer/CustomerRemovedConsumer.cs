using Acropolis.Consumer.Common;
using Acropolis.Consumer.Features.Customer.Messages;
using DotNetCore.CAP;
using Ziggurat;

namespace Acropolis.Consumer.Features.Customer.Consumer;

public class CustomerRemovedConsumer : ICapSubscribe
{
    private readonly IConsumerService<CustomerRemovedMessage> _consumerServiceCustomerRemoved;

    public CustomerRemovedConsumer(IConsumerService<CustomerRemovedMessage> consumerServiceCenterRemoved)
        => _consumerServiceCustomerRemoved = consumerServiceCenterRemoved;

    [CapSubscribe(RouterKeys.CustomerRemoved, Group = RouterKeys.GroupCustomerRemoved)]
    public async Task CustomerRemovedSubscription(CustomerRemovedMessage message)
        => await _consumerServiceCustomerRemoved.ProcessMessageAsync(message);
}