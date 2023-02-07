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

public class ProductUpdatedConsumerTests : TestProgram<Program>
{
    private readonly Mock<ILogger<ProductUpdatedService>> _logger;
    private readonly ProductUpdatedConsumer _consumer;
    private readonly ProductUpdatedService _service;
    private readonly AcropolisContext _context;

    public ProductUpdatedConsumerTests()
    {
        _logger = new Mock<ILogger<ProductUpdatedService>>();
        _context = Server.Services.GetRequiredService<AcropolisContext>();
        _service = BuildService();
        _consumer = new ProductUpdatedConsumer(_service);
    }

    [Fact(DisplayName = "Should Product UpdatedSubscription with success")]
    public async Task Should_UpdatedSubscription_WithSuccess()
    {
        // Arrange
        var product = await SqlTestData.ProductSeeder(Server.Services.CreateScope());

        var message = new ProductUpdatedMessage(Guid.NewGuid().ToString(), RouterKeys.GroupProductUpdated)
        {
            Id = product.Id,
            MaterialCode = "123456",
            Name = "Produto Teste Name Alterado",
            SellerId = product.SellerId,
            Status = 2,
            UnitMeasure = "Ml",
            UnitWeight = "2",
            Weight = 20
        };

        // Act
        Func<Task> act = async () => await _consumer.ProductUpdatedSubscription(message);

        // Assert
        await act.Should()
            .NotThrowAsync();

        var productDb = _context.Products.FirstOrDefault(x => x.Id == message.Id);

        productDb.Should().NotBeNull();
        productDb?.Name.Should().Be(message.Name);

        // Assert Log
        _logger.VerifyLogging("", LogLevel.Error, Times.Never());
    }

    [Fact(DisplayName = "Should Product UpdatedSubscription with validation error")]
    public async Task Should_UpdatedSubscription_WithValidationError()
    {
        // Arrange
        var message = new ProductUpdatedMessage(Guid.NewGuid().ToString(), RouterKeys.GroupProductUpdated)
        {
            Id = Guid.NewGuid(),
            MaterialCode = "123456",
            Name = "",
            SellerId = Guid.NewGuid(),
            Status = 1,
            UnitMeasure = "Ml",
            UnitWeight = "2",
            Weight = 20
        };

        var validator = new UpdateProductValidation();
        var validatorResult = await validator.ValidateAsync(message);
        var loggerMessage = string.Format("Message validation error: {0}.\n Errors: {1}",
            nameof(ProductUpdatedMessage),
            JsonConvert.SerializeObject(validatorResult
                .Errors
                .Select(x => x.ErrorMessage)
                .ToArray()));

        // Act
        Func<Task> act = async () => await _consumer.ProductUpdatedSubscription(message);

        // Assert
        await act.Should()
            .NotThrowAsync();

        _logger.VerifyLogging(loggerMessage, LogLevel.Error);
    }

    [Fact(DisplayName = "Should Product UpdatedSubscription with success when product exists")]
    public async Task Should_UpdatedSubscription_WithSuccessWhenProductExists()
    {
        // Arrange
        var product = await SqlTestData.ProductSeeder(Server.Services.CreateScope());

        var message = new ProductUpdatedMessage(Guid.NewGuid().ToString(), RouterKeys.GroupProductUpdated)
        {
            Id = Guid.NewGuid(),
            MaterialCode = "123456",
            Name = "Produto Teste Name",
            SellerId = product.SellerId,
            Status = 1,
            UnitMeasure = "Ml",
            UnitWeight = "2",
            Weight = 20
        };

        // Act
        Func<Task> act = async () => await _consumer.ProductUpdatedSubscription(message);

        // Assert
        await act.Should()
             .NotThrowAsync();
    }

    private ProductUpdatedService BuildService()
    {
        var _loggerProductAppService = new Mock<ILogger<ProductAppService>>();

        var _unitOfWork = new UnitOfWork(_context);
        var _productRepository = new ProductRepository(_context);
        var _productAppService = new ProductAppService(_unitOfWork, _productRepository, _loggerProductAppService.Object);
        var _mapper = new MapperConfiguration(cfg => cfg.AddProfile<ConfiguringMapperProfile>()).CreateMapper();

        return new ProductUpdatedService(_productAppService, _logger.Object, _mapper);
    }
}