using Acropolis.Consumer.Common;
using Acropolis.Consumer.Features.Customer.Messages;
using DotNetCore.CAP;
using Ziggurat;

namespace Acropolis.Consumer.Features.Customer.Consumer;

public class CustomerChangedConsumer : ICapSubscribe
{
    private readonly IConsumerService<CustomerChangedMessage> _consumerServiceCustomerChanged;

    public CustomerChangedConsumer(IConsumerService<CustomerChangedMessage> consumerServiceCenterChanged)
        => _consumerServiceCustomerChanged = consumerServiceCenterChanged;

    [CapSubscribe(RouterKeys.CustomerChanged, Group = RouterKeys.GroupCustomerChanged)]
    public async Task CustomerChangedSubscription(CustomerChangedMessage message)
        => await _consumerServiceCustomerChanged.ProcessMessageAsync(message);
}