using Acropolis.Application.Features.Products.Services;
using Acropolis.Application.Features.Products.Validators;
using Acropolis.Consumer.Common;
using Acropolis.Consumer.Common.AutoMapper;
using Acropolis.Consumer.Features.Product.Consumer;
using Acropolis.Consumer.Features.Product.Messages;
using Acropolis.Consumer.Features.Product.Services;
using Acropolis.Tests.Helpers;
using Acropolis.Tests.Consumer.Support.Data;
using Acropolis.Infrastructure.Contexts;
using Acropolis.Infrastructure.Contexts.Persistence;
using Acropolis.Infrastructure.Repositories;
using AutoMapper;
using DotNetCore.CAP;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;

namespace Acropolis.Tests.Consumer.Unit.Product;

public class ProductRemovedConsumerTests : TestProgram<Program>
{
    private readonly Mock<ILogger<ProductRemovedService>> _logger;
    private readonly ProductRemovedService _service;
    private readonly ProductRemovedConsumer _consumer;
    private readonly AcropolisContext _context;

    public ProductRemovedConsumerTests()
    {
        _logger = new Mock<ILogger<ProductRemovedService>>();
        _context = Server.Services.GetRequiredService<AcropolisContext>();
        _service = BuildService();
        _consumer = new ProductRemovedConsumer(_service);
    }

    [Fact(DisplayName = "Should Product RemovedSubscription with success")]
    public async Task Should_RemovedSubscription_WithSuccess()
    {
        // Arrange
        var product = await SqlTestData.ProductSeeder(Server.Services.CreateScope());

        var message = new ProductRemovedMessage(Guid.NewGuid().ToString(), RouterKeys.GroupProductRemoved)
        {
            Id = product.Id
        };

        // Act
        Func<Task> act = async () => await _consumer.ProductRemovedSubscription(message);

        // Assert
        await act.Should()
            .NotThrowAsync();

        var productDb = _context.Products.FirstOrDefault(p => p.Id == message.Id);
        productDb.Should().BeNull();

        // Assert Log
        _logger.VerifyLogging("", LogLevel.Error, Times.Never());
    }

    [Fact(DisplayName = "Should Product RemovedSubscription with validation error")]
    public async Task Should_RemovedSubscription_WithValidationError()
    {
        // Arrange
        var message = new ProductRemovedMessage(Guid.NewGuid().ToString(), RouterKeys.GroupProductRemoved)
        {
            Id = Guid.Empty
        };

        var validator = new RemoveProductValidation();
        var validatorResult = await validator.ValidateAsync(message);
        var loggerMessage = string.Format("Message validation error: {0}.\n Errors: {1}",
            nameof(ProductRemovedMessage),
            JsonConvert.SerializeObject(validatorResult
                .Errors
                .Select(x => x.ErrorMessage)
                .ToArray()));

        // Act
        Func<Task> act = async () => await _consumer.ProductRemovedSubscription(message);

        // Assert
        await act.Should()
            .NotThrowAsync();

        _logger.VerifyLogging(loggerMessage, LogLevel.Error);
    }

    [Fact(DisplayName = "Should Product RemovedSubscription with error")]
    public async Task Should_RemovedSubscription_WithError()
    {
        // Arrange
        await SqlTestData.ProductSeeder(Server.Services.CreateScope());

        var message = new ProductRemovedMessage(Guid.NewGuid().ToString(), RouterKeys.GroupProductRemoved)
        {
            Id = Guid.NewGuid()
        };

        // Act
        Func<Task> act = async () => await _consumer.ProductRemovedSubscription(message);

        // Assert
        await act.Should()
            .ThrowExactlyAsync<SubscriberNotFoundException>();
    }

    private ProductRemovedService BuildService()
    {
        var _loggerProductAppService = new Mock<ILogger<ProductAppService>>();

        var _unitOfWork = new UnitOfWork(_context);
        var _productRepository = new ProductRepository(_context);
        var _productAppService = new ProductAppService(_unitOfWork, _productRepository, _loggerProductAppService.Object);
        var _mapper = new MapperConfiguration(cfg => cfg.AddProfile<ConfiguringMapperProfile>()).CreateMapper();

        return new ProductRemovedService(_productAppService, _logger.Object, _mapper);
    }
}