using Acropolis.Application.Features.Attributes.Enums;
using Acropolis.Application.Features.Attributes.Services;
using Acropolis.Application.Features.Attributes.Validators;
using Acropolis.Consumer.Attribute.Services;
using Acropolis.Consumer.Common;
using Acropolis.Consumer.Common.AutoMapper;
using Acropolis.Consumer.Features.Attribute.Consumer;
using Acropolis.Consumer.Features.Attribute.Messages;
using Acropolis.Tests.Helpers;
using Acropolis.Infrastructure.Contexts;
using Acropolis.Infrastructure.Contexts.Persistence;
using Acropolis.Infrastructure.Repositories;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;

namespace Acropolis.Tests.Consumer.Unit.Attribute;

public class AttributeCreatedConsumerTests : TestProgram<Program>
{
    private readonly Mock<ILogger<AttributeCreatedService>> _logger;
    private readonly AttributeCreatedConsumer _consumer;
    private readonly AcropolisContext _context;

    public AttributeCreatedConsumerTests()
    {
        _logger = new Mock<ILogger<AttributeCreatedService>>();
        _context = Server.Services.GetRequiredService<AcropolisContext>();
        _consumer = BuildConsumer();
    }

    [Fact(DisplayName = "Should attribute createdSubscription with success")]
    public async Task Should_CreatedSubscription_WithSuccess()
    {
        // Arrange
        var message = new AttributeCreatedMessage(Guid.NewGuid().ToString(), RouterKeys.GroupCatalogAttributeUpdated)
        {
            ProductId = Guid.NewGuid(),
            AttributeKeyId = Guid.NewGuid(),
            AttributeKeyDescription = "Capacidade",
            AttributeKeyIsBeginOpen = false,
            AttributeKey = "capacidade",
            AttributeKeyLabel = "Capacidade",
            AttributeKeyStatus = 1,
            AttributeKeyType = FilterType.Multi,
            AttributeValueId = Guid.NewGuid(),
            AttributeValueLabel = "4 kg",
            AttributeValueStatus = 1,
            AttributeValue = "4-kg"
        };

        // Act
        Func<Task> act = async () => await _consumer.AttributeCreatedSubscription(message);

        // Assert
        await act.Should()
            .NotThrowAsync();

        var attribute = _context.Attributes.First();
        attribute.Should().NotBeNull();
        attribute.AttributeKey.Should().Be(message.AttributeKey);

        // Assert Log
        _logger.VerifyLogging("", LogLevel.Error, Times.Never());
    }

    [Fact(DisplayName = "Should attribute createdSubscription with error")]
    public async Task Should_CreatedSubscription_WithError()
    {
        // Arrange
        var message = new AttributeCreatedMessage(Guid.NewGuid().ToString(), RouterKeys.GroupCatalogAttributeUpdated)
        {
            ProductId = Guid.NewGuid(),
            AttributeKeyId = Guid.NewGuid(),
            AttributeKeyDescription = string.Empty,
            AttributeKeyIsBeginOpen = false,
            AttributeKey = string.Empty,
            AttributeKeyLabel = string.Empty,
            AttributeKeyStatus = 1,
            AttributeKeyType = FilterType.Multi,
            AttributeValueId = Guid.Empty,
            AttributeValueLabel = string.Empty,
            AttributeValueStatus = 1,
            AttributeValue = string.Empty
        };

        var validator = new CreateAttributeValidation();
        var validatorResult = await validator.ValidateAsync(message);
        var loggerMessage = string.Format("Message validation error: {0}.\n Errors: {1}",
            nameof(AttributeCreatedMessage),
            JsonConvert.SerializeObject(validatorResult
                .Errors
                .Select(x => x.ErrorMessage)
                .ToArray()));

        // Act
        Func<Task> act = async () => await _consumer.AttributeCreatedSubscription(message);

        // Assert
        await act.Should()
            .NotThrowAsync();

        _logger.VerifyLogging(loggerMessage, LogLevel.Error);
    }

    private AttributeCreatedConsumer BuildConsumer()
    {
        var _loggerAttributeAppService = new Mock<ILogger<AttributeAppService>>();

        var _unitOfWork = new UnitOfWork(_context);
        var _attributeRepository = new AttributeRepository(_context);
        var _attributeAppService = new AttributeAppService(_unitOfWork, _attributeRepository, _loggerAttributeAppService.Object);
        var _mapper = new MapperConfiguration(cfg => cfg.AddProfile<ConfiguringMapperProfile>()).CreateMapper();

        var service = new AttributeCreatedService(_attributeAppService, _logger.Object, _mapper);

        return new AttributeCreatedConsumer(service);
    }
}