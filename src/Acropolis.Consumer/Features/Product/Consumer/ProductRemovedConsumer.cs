using Acropolis.Consumer.Common;
using Acropolis.Consumer.Features.Product.Messages;
using DotNetCore.CAP;
using Ziggurat;

namespace Acropolis.Consumer.Features.Product.Consumer;

public class ProductRemovedConsumer : ICapSubscribe
{
    private readonly IConsumerService<ProductRemovedMessage> _consumerService;

    public ProductRemovedConsumer(IConsumerService<ProductRemovedMessage> consumerService)
        => _consumerService = consumerService;

    [CapSubscribe(RouterKeys.CatalogProductRemoved, Group = RouterKeys.GroupProductRemoved)]
    public async Task ProductRemovedSubscription(ProductRemovedMessage message)
        => await _consumerService.ProcessMessageAsync(message);
}