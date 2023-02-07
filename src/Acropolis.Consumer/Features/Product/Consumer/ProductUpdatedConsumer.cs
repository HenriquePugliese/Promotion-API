using Acropolis.Consumer.Common;
using Acropolis.Consumer.Features.Product.Messages;
using DotNetCore.CAP;
using Ziggurat;

namespace Acropolis.Consumer.Features.Product.Consumer;

public class ProductUpdatedConsumer : ICapSubscribe
{
    private readonly IConsumerService<ProductUpdatedMessage> _consumerService;

    public ProductUpdatedConsumer(IConsumerService<ProductUpdatedMessage> consumerService)
        => _consumerService = consumerService;

    [CapSubscribe(RouterKeys.CatalogProductUpdated, Group = RouterKeys.GroupProductUpdated)]
    public async Task ProductUpdatedSubscription(ProductUpdatedMessage message)
        => await _consumerService.ProcessMessageAsync(message);
}