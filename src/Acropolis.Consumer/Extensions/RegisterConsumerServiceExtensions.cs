using Acropolis.Consumer.Attribute.Services;
using Acropolis.Consumer.Common;
using Acropolis.Consumer.Features.Attribute.Consumer;
using Acropolis.Consumer.Features.Attribute.Messages;
using Acropolis.Consumer.Features.Customer.Consumer;
using Acropolis.Consumer.Features.Customer.Messages;
using Acropolis.Consumer.Features.Customer.Services;
using Acropolis.Consumer.Features.Product.Consumer;
using Acropolis.Consumer.Features.Product.Messages;
using Acropolis.Consumer.Features.Product.Services;
using Acropolis.Consumer.Middlewares;
using Acropolis.Infrastructure.Contexts;
using DotNetCore.CAP;
using Ziggurat;

namespace Acropolis.Consumer.Extensions;

public static class RegisterConsumerServiceExtensions
{
    public static IServiceCollection RegisterConsumerServiceResolver(this IServiceCollection services, string consumerArg)
        => consumerArg switch
        {
            ConsumerArgNames.AttributeUpdatedConsumer =>
                services.ConsumerServiceResolver<AttributeCreatedConsumer, AttributeCreatedMessage, AttributeCreatedService>(),
            ConsumerArgNames.ProductChangedConsumer =>
                services.ConsumerServiceResolver<ProductChangedConsumer, ProductChangedMessage, ProductChangedService>(),
            ConsumerArgNames.ProductCreatedConsumer =>
                services.ConsumerServiceResolver<ProductCreatedConsumer, ProductCreatedMessage, ProductCreatedService>(),
            ConsumerArgNames.ProductRemovedConsumer =>
                services.ConsumerServiceResolver<ProductRemovedConsumer, ProductRemovedMessage, ProductRemovedService>(),
            ConsumerArgNames.ProductUpdatedConsumer =>
                services.ConsumerServiceResolver<ProductUpdatedConsumer, ProductUpdatedMessage, ProductUpdatedService>(),
            ConsumerArgNames.CustomerChangedConsumer =>
                services.ConsumerServiceResolver<CustomerChangedConsumer, CustomerChangedMessage, CustomerChangedService>(),
            ConsumerArgNames.CustomerCreatedConsumer =>
                services.ConsumerServiceResolver<CustomerCreatedConsumer, CustomerCreatedMessage, CustomerCreatedService>(),
            ConsumerArgNames.CustomerRemovedConsumer =>
                services.ConsumerServiceResolver<CustomerRemovedConsumer, CustomerRemovedMessage, CustomerRemovedService>(),
            _ => throw new ArgumentException($"Consumer arg name {consumerArg} not found.")
        };

    public static IServiceCollection ConsumerServiceResolver<TCapSubscribe, TMessage, TConsumerService>(this IServiceCollection services)
        where TCapSubscribe : class, ICapSubscribe
        where TMessage : IMessage
        where TConsumerService : class, IConsumerService<TMessage>
        => services.AddScoped<TCapSubscribe>()
            .AddConsumerService<TMessage, TConsumerService>(options
                =>
            {
                options.Use<LoggingMiddleware<TMessage>>();
                options.UseEntityFrameworkIdempotency<TMessage, AcropolisContext>();
            });
}