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

public class ProductCreatedConsumerTests : TestProgram<Program>
{
    private readonly Mock<ILogger<ProductCreatedService>> _logger;
    private readonly ProductCreatedService _service;
    private readonly ProductCreatedConsumer _consumer;
    private readonly AcropolisContext _context;

    public ProductCreatedConsumerTests()
    {
        _logger = new Mock<ILogger<ProductCreatedService>>();
        _context = Server.Services.GetRequiredService<AcropolisContext>();
        _service = BuildService();
        _consumer = new ProductCreatedConsumer(_service);
    }

    [Fact(DisplayName = "Should Product createdSubscription with success")]
    public async Task Should_CreatedSubscription_WithSuccess()
    {
        // Arrange
        var message = new ProductCreatedMessage(Guid.NewGuid().ToString(), RouterKeys.GroupProductCreated)
        {
            Id = Guid.NewGuid(),
            MaterialCode = "123456",
            Name = "Produto Teste Name",
            SellerId = Guid.NewGuid(),
            Status = 1,
            UnitMeasure = "Kg",
            UnitWeight = "1",
            Weight = 10
        };

        // Act
        Func<Task> act = async () => await _consumer.ProductCreatedSubscription(message);

        // Assert
        await act.Should()
            .NotThrowAsync();

        var product = _context.Products.FirstOrDefault(x => x.Id == message.Id);
        product.Should().NotBeNull();
        if (product != null)
            product.Id.Should().Be(message.Id);

        // Assert Log
        _logger.VerifyLogging("", LogLevel.Error, Times.Never());
    }

    [Fact(DisplayName = "Should Product createdSubscription with validation error")]
    public async Task Should_CreatedSubscription_WithValidationError()
    {
        // Arrange
        var message = new ProductCreatedMessage(Guid.NewGuid().ToString(), RouterKeys.GroupProductCreated)
        {
            Id = Guid.NewGuid(),
            MaterialCode = "123456",
            Name = "",
            SellerId = Guid.NewGuid(),
            Status = 1,
            UnitMeasure = "Kg",
            UnitWeight = "1",
            Weight = 10
        };

        var validator = new CreateProductValidation();
        var validatorResult = await validator.ValidateAsync(message);
        var loggerMessage = string.Format("Message validation error: {0}.\n Errors: {1}",
            nameof(ProductCreatedMessage),
            JsonConvert.SerializeObject(validatorResult
                .Errors
                .Select(x => x.ErrorMessage)
                .ToArray()));

        // Act
        Func<Task> act = async () => await _consumer.ProductCreatedSubscription(message);

        // Assert
        await act.Should()
            .NotThrowAsync();

        _logger.VerifyLogging(loggerMessage, LogLevel.Error);
    }

    [Fact(DisplayName = "Should Product CreatedSubscription with error")]
    public async Task Should_CreatedSubscription_WithError()
    {
        // Arrange
        var product = await SqlTestData.ProductSeeder(Server.Services.CreateScope());

        var message = new ProductCreatedMessage(Guid.NewGuid().ToString(), RouterKeys.GroupProductCreated)
        {
            Id = product.Id,
            MaterialCode = "123456",
            Name = "Produto Teste Name",
            SellerId = product.SellerId,
            Status = 1,
            UnitMeasure = "Ml",
            UnitWeight = "2",
            Weight = 20
        };

        // Act
        Func<Task> act = async () => await _consumer.ProductCreatedSubscription(message);

        // Assert
        await act.Should()
            .ThrowExactlyAsync<SubscriberNotFoundException>();
    }

    private ProductCreatedService BuildService()
    {
        var _loggerProductAppService = new Mock<ILogger<ProductAppService>>();

        var _unitOfWork = new UnitOfWork(_context);
        var _productRepository = new ProductRepository(_context);
        var _productAppService = new ProductAppService(_unitOfWork, _productRepository, _loggerProductAppService.Object);
        var _mapper = new MapperConfiguration(cfg => cfg.AddProfile<ConfiguringMapperProfile>()).CreateMapper();

        return new ProductCreatedService(_productAppService, _logger.Object, _mapper);
    }
}