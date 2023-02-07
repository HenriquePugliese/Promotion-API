using Acropolis.Consumer.Common;
using Acropolis.Consumer.Features.Product.Messages;
using DotNetCore.CAP;
using Ziggurat;

namespace Acropolis.Consumer.Features.Product.Consumer;

public class ProductCreatedConsumer : ICapSubscribe
{
    private readonly IConsumerService<ProductCreatedMessage> _consumerService;

    public ProductCreatedConsumer(IConsumerService<ProductCreatedMessage> consumerService)
        => _consumerService = consumerService;

    [CapSubscribe(RouterKeys.CatalogProductCreated, Group = RouterKeys.GroupProductCreated)]
    public async Task ProductCreatedSubscription(ProductCreatedMessage message)
        => await _consumerService.ProcessMessageAsync(message);
}