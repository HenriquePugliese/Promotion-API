using Acropolis.Consumer.Common;
using Acropolis.Consumer.Features.Customer.Messages;
using DotNetCore.CAP;
using Ziggurat;

namespace Acropolis.Consumer.Features.Customer.Consumer;

public class CustomerCreatedConsumer : ICapSubscribe
{
    private readonly IConsumerService<CustomerCreatedMessage> _consumerServiceCustomerCreated;

    public CustomerCreatedConsumer(IConsumerService<CustomerCreatedMessage> consumerServiceCenterCreated)
        => _consumerServiceCustomerCreated = consumerServiceCenterCreated;

    [CapSubscribe(RouterKeys.CustomerCreated, Group = RouterKeys.GroupCustomerCreated)]
    public async Task CustomerCreatedSubscription(CustomerCreatedMessage message)
        => await _consumerServiceCustomerCreated.ProcessMessageAsync(message);
}