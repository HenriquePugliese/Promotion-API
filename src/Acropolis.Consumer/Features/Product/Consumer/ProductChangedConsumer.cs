using Acropolis.Consumer.Common;
using Acropolis.Consumer.Features.Product.Messages;
using DotNetCore.CAP;
using Ziggurat;

namespace Acropolis.Consumer.Features.Product.Consumer;

public class ProductChangedConsumer : ICapSubscribe
{
    private readonly IConsumerService<ProductChangedMessage> _consumerService;

    public ProductChangedConsumer(IConsumerService<ProductChangedMessage> consumerService)
        => _consumerService = consumerService;

    [CapSubscribe(RouterKeys.CatalogProductChanged, Group = RouterKeys.GroupProductChanged)]
    public async Task ProductChangedSubscription(ProductChangedMessage message)
        => await _consumerService.ProcessMessageAsync(message);
}