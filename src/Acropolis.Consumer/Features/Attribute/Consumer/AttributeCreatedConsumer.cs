using Acropolis.Consumer.Common;
using Acropolis.Consumer.Features.Attribute.Messages;
using DotNetCore.CAP;
using Ziggurat;

namespace Acropolis.Consumer.Features.Attribute.Consumer;

public class AttributeCreatedConsumer : ICapSubscribe
{
    private readonly IConsumerService<AttributeCreatedMessage> _consumerServiceAttributeCreated;

    public AttributeCreatedConsumer(IConsumerService<AttributeCreatedMessage> consumerServiceCenterCreated)
        => _consumerServiceAttributeCreated = consumerServiceCenterCreated;

    [CapSubscribe(RouterKeys.CatalogAttributeUpdated, Group = RouterKeys.GroupCatalogAttributeUpdated)]
    public async Task AttributeCreatedSubscription(AttributeCreatedMessage message)
        => await _consumerServiceAttributeCreated.ProcessMessageAsync(message);
}