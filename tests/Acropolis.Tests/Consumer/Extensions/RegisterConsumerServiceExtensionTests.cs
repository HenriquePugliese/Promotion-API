using Acropolis.Consumer.Common;
using Acropolis.Consumer.Extensions;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace Acropolis.Tests.Consumer.Extensions;

public class RegisterConsumerServiceExtensionTests
{
    private readonly IServiceCollection _services;
    public RegisterConsumerServiceExtensionTests()
    {
        //Arrange
        _services = new ServiceCollection();
    }

    [Theory(DisplayName = "Should throw error ArgumentException error when argument not matching any defined or null or empty")]
    [InlineData("anyarg")]
    [InlineData("")]
    [InlineData(null)]
    public void CapConsumerStartup_RegisterConsumerServiceResolverWithNoMatchingArg_ShouldThrowArgumentExceptionError(string consumerArg)
    {
        //Act
        try
        {
            _services.RegisterConsumerServiceResolver(consumerArg);
        }
        catch (Exception ex)
        {
            //Assert
            ex.Should().BeOfType<ArgumentException>();
            ex.Message.Should().Be($"Consumer arg name {consumerArg} not found.");
        }
    }

    [Fact(DisplayName = "Should resolve when customer-created-consumer argument matching")]
    public void CapConsumerStartup_RegisterConsumerServiceResolverMatchingCustomerCreatedConsumer_ShouldResolve()
    {
        //Act
        var serviceAct = _services.RegisterConsumerServiceResolver(ConsumerArgNames.CustomerCreatedConsumer);

        //Assert
        serviceAct.Should().NotBeNullOrEmpty();
    }

    [Fact(DisplayName = "Should resolve when customer-changed-consumer argument matching")]
    public void CapConsumerStartup_RegisterConsumerServiceResolverMatchingCustomerChangedConsumer_ShouldResolve()
    {
        //Act
        var serviceAct = _services.RegisterConsumerServiceResolver(ConsumerArgNames.CustomerChangedConsumer);

        //Assert
        serviceAct.Should().NotBeNullOrEmpty();
    }

    [Fact(DisplayName = "Should resolve when customer-removed-consumer argument matching")]
    public void CapConsumerStartup_RegisterConsumerServiceResolverMatchingCustomerRemovedConsumer_ShouldResolve()
    {
        //Act
        var serviceAct = _services.RegisterConsumerServiceResolver(ConsumerArgNames.CustomerRemovedConsumer);

        //Assert
        serviceAct.Should().NotBeNullOrEmpty();
    }

    [Fact(DisplayName = "Should resolve when product-created-consumer argument matching")]
    public void CapConsumerStartup_RegisterConsumerServiceResolverMatchingProductCreatedConsumer_ShouldResolve()
    {
        //Act
        var serviceAct = _services.RegisterConsumerServiceResolver(ConsumerArgNames.ProductCreatedConsumer);

        //Assert
        serviceAct.Should().NotBeNullOrEmpty();
    }

    [Fact(DisplayName = "Should resolve when product-updated-consumer argument matching")]
    public void CapConsumerStartup_RegisterConsumerServiceResolverMatchingProductUpdatedConsumer_ShouldResolve()
    {
        //Act
        var serviceAct = _services.RegisterConsumerServiceResolver(ConsumerArgNames.ProductUpdatedConsumer);

        //Assert
        serviceAct.Should().NotBeNullOrEmpty();
    }

    [Fact(DisplayName = "Should resolve when product-changed-consumer argument matching")]
    public void CapConsumerStartup_RegisterConsumerServiceResolverMatchingProductChangedConsumer_ShouldResolve()
    {
        //Act
        var serviceAct = _services.RegisterConsumerServiceResolver(ConsumerArgNames.ProductChangedConsumer);

        //Assert
        serviceAct.Should().NotBeNullOrEmpty();
    }

    [Fact(DisplayName = "Should resolve when product-removed-consumer argument matching")]
    public void CapConsumerStartup_RegisterConsumerServiceResolverMatchingProductRemovedConsumer_ShouldResolve()
    {
        //Act
        var serviceAct = _services.RegisterConsumerServiceResolver(ConsumerArgNames.ProductRemovedConsumer);

        //Assert
        serviceAct.Should().NotBeNullOrEmpty();
    }

    [Fact(DisplayName = "Should resolve when attribute-updated-consumer argument matching")]
    public void CapConsumerStartup_RegisterConsumerServiceResolverMatchingAttributeUpdatedConsumer_ShouldResolve()
    {
        //Act
        var serviceAct = _services.RegisterConsumerServiceResolver(ConsumerArgNames.AttributeUpdatedConsumer);

        //Assert
        serviceAct.Should().NotBeNullOrEmpty();
    }
}
