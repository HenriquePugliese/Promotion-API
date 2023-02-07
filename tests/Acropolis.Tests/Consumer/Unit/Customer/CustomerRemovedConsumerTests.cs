using Acropolis.Application.Features.Customers.Services;
using Acropolis.Application.Features.Customers.Validators;
using Acropolis.Consumer.Common;
using Acropolis.Consumer.Common.AutoMapper;
using Acropolis.Consumer.Features.Customer.Consumer;
using Acropolis.Consumer.Features.Customer.Messages;
using Acropolis.Consumer.Features.Customer.Services;
using Acropolis.Tests.Helpers;
using Acropolis.Tests.Consumer.Support.Data;
using Acropolis.Infrastructure.Contexts;
using Acropolis.Infrastructure.Contexts.Persistence;
using Acropolis.Infrastructure.Repositories;
using AutoMapper;
using Bogus;
using Bogus.Extensions.Brazil;
using DotNetCore.CAP;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;

namespace Acropolis.Tests.Consumer.Unit.Customer;

public class CustomerRemovedConsumerTests : TestProgram<Program>
{
    private readonly Mock<ILogger<CustomerRemovedService>> _logger;
    private readonly CustomerRemovedService _service;
    private readonly CustomerRemovedConsumer _consumer;
    private readonly AcropolisContext _context;

    public CustomerRemovedConsumerTests()
    {
        _logger = new Mock<ILogger<CustomerRemovedService>>();
        _context = Server.Services.GetRequiredService<AcropolisContext>();
        _service = BuildService();
        _consumer = new CustomerRemovedConsumer(_service);
    }

    [Fact(DisplayName = "Should Customer RemovedSubscription with success")]
    public async Task Should_RemovedSubscription_WithSuccess()
    {
        // Arrange
        var customer = await SqlTestData.CustomerSeeder(Server.Services.CreateScope());

        var message = new CustomerRemovedMessage(Guid.NewGuid().ToString(), RouterKeys.GroupCustomerRemoved)
        {
            Cnpj = customer.Cnpj,
            SellerId = customer.SellerId,
        };

        // Act
        Func<Task> act = async () => await _consumer.CustomerRemovedSubscription(message);

        // Assert
        await act.Should()
            .NotThrowAsync();

        var customerDb = _context.Customers.FirstOrDefault(p => p.Cnpj == message.Cnpj && p.SellerId == message.SellerId);
        customerDb.Should().BeNull();

        // Assert Log
        _logger.VerifyLogging("", LogLevel.Error, Times.Never());
    }

    [Fact(DisplayName = "Should Customer RemovedSubscription with validation error")]
    public async Task Should_RemovedSubscription_WithValidationError()
    {
        // Arrange
        var message = new CustomerRemovedMessage(Guid.NewGuid().ToString(), RouterKeys.GroupCustomerRemoved)
        {
            Cnpj = string.Empty,
        };

        var validator = new RemoveCustomerValidation();
        var validatorResult = await validator.ValidateAsync(message);
        var loggerMessage = string.Format("Message validation error: {0}.\n Errors: {1}",
            nameof(CustomerRemovedMessage),
            JsonConvert.SerializeObject(validatorResult
                .Errors
                .Select(x => x.ErrorMessage)
                .ToArray()));

        // Act
        Func<Task> act = async () => await _consumer.CustomerRemovedSubscription(message);

        // Assert
        await act.Should()
            .NotThrowAsync();

        _logger.VerifyLogging(loggerMessage, LogLevel.Error);
    }

    [Fact(DisplayName = "Should Customer RemovedSubscription with error")]
    public async Task Should_RemovedSubscription_WithError()
    {
        // Arrange
        var message = new CustomerRemovedMessage(Guid.NewGuid().ToString(), RouterKeys.GroupCustomerRemoved)
        {
            Cnpj = new Faker("pt_BR").Company.Cnpj(),
        };

        // Act
        Func<Task> act = async () => await _consumer.CustomerRemovedSubscription(message);

        // Assert
        await act.Should()
            .ThrowExactlyAsync<SubscriberNotFoundException>();
    }

    private CustomerRemovedService BuildService()
    {
        var _loggerCustomerAppService = new Mock<ILogger<CustomerAppService>>();

        var _unitOfWork = new UnitOfWork(_context);
        var _customerRepository = new CustomerRepository(_context);
        var _parameterRepository = new ParameterRepository(_context);
        var _customerAppService = new CustomerAppService(_unitOfWork, _customerRepository, _parameterRepository, _loggerCustomerAppService.Object);
        var _mapper = new MapperConfiguration(cfg => cfg.AddProfile<ConfiguringMapperProfile>()).CreateMapper();

        return new CustomerRemovedService(_customerAppService, _logger.Object, _mapper);
    }
}