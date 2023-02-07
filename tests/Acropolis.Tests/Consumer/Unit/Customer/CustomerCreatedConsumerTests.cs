using Acropolis.Application.Features.Customers.Services;
using Acropolis.Application.Features.Customers.Validators;
using Acropolis.Application.Features.Parameters.Requests;
using Acropolis.Consumer.Common;
using Acropolis.Consumer.Common.AutoMapper;
using Acropolis.Consumer.Features.Customer.Consumer;
using Acropolis.Consumer.Features.Customer.Messages;
using Acropolis.Consumer.Features.Customer.Services;
using Acropolis.Tests.Helpers;
using Acropolis.Infrastructure.Contexts;
using Acropolis.Infrastructure.Contexts.Persistence;
using Acropolis.Infrastructure.Repositories;
using AutoMapper;
using Bogus;
using Bogus.Extensions.Brazil;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;

namespace Acropolis.Tests.Consumer.Unit.Customer;

public class CustomerCreatedConsumerTests : TestProgram<Program>
{
    private readonly Mock<ILogger<CustomerCreatedService>> _logger;
    private readonly CustomerCreatedConsumer _consumer;
    private readonly AcropolisContext _context;

    public CustomerCreatedConsumerTests()
    {
        _logger = new Mock<ILogger<CustomerCreatedService>>();
        _context = Server.Services.GetRequiredService<AcropolisContext>();
        _consumer = BuildService();
    }

    [Fact(DisplayName = "Should Customer createdSubscription with success")]
    public async Task Should_CreatedSubscription_WithSuccess()
    {
        // Arrange
        var message = new CustomerCreatedMessage(Guid.NewGuid().ToString(), RouterKeys.GroupCustomerCreated)
        {
            Cnpj = new Faker("pt_BR").Company.Cnpj(),
            SellerId = Guid.NewGuid().ToString(),
            CustomerCode = "987654",
            Parameters = new List<ParameterRequest>() {
                new()
                {
                    Description = "VC-SP-ITAPETININGA",
                    Code = "Mesoregiao" ,
                    Value = "35B" ,
                    Status = true
                },
                new()
                {
                    Description = "VC-TATUI" ,
                    Code = "Microregiao" ,
                    Value = "3B8" ,
                    Status = true
                },
            }
        };

        // Act
        Func<Task> act = async () => await _consumer.CustomerCreatedSubscription(message);

        // Assert
        await act.Should()
            .NotThrowAsync();

        var customer = _context.Customers.FirstOrDefault(x => x.Cnpj == message.Cnpj && x.SellerId == message.SellerId);

        customer.Should().NotBeNull();

        // Assert Log
        _logger.VerifyLogging("", LogLevel.Error, Times.Never());
    }

    [Fact(DisplayName = "Should Customer createdSubscription with error")]
    public async Task Should_CreatedSubscription_WithError()
    {
        // Arrange
        var message = new CustomerCreatedMessage(Guid.NewGuid().ToString(), RouterKeys.GroupCustomerCreated)
        {
            Cnpj = string.Empty,
            Parameters = new List<ParameterRequest>() {
                new()
                {
                    Description = string.Empty,
                    Code = "Mesoregiao",
                    Value = "35B",
                    Status = true
                },
                new()
                {
                    Description = "VC-TATUI",
                    Code = string.Empty,
                    Value = "3B8",
                    Status = true
                },
                new()
                {
                    Description = "VC-DISTRIBUIDOR",
                    Code = "GrupoSegmento",
                    Value = string.Empty,
                    Status = true
                }
            }
        };

        var validator = new CreateCustomerValidation();
        var validatorResult = await validator.ValidateAsync(message);
        var loggerMessage = string.Format("Message validation error: {0}.\n Errors: {1}",
            nameof(CustomerCreatedMessage),
            JsonConvert.SerializeObject(validatorResult
                .Errors
                .Select(x => x.ErrorMessage)
                .ToArray()));

        // Act
        Func<Task> act = async () => await _consumer.CustomerCreatedSubscription(message);

        // Assert
        await act.Should()
            .NotThrowAsync();

        _logger.VerifyLogging(loggerMessage, LogLevel.Error);
    }

    private CustomerCreatedConsumer BuildService()
    {
        var _loggerCustomerAppService = new Mock<ILogger<CustomerAppService>>();

        var _unitOfWork = new UnitOfWork(_context);
        var _customerRepository = new CustomerRepository(_context);
        var _parameterRepository = new ParameterRepository(_context);
        var _customerAppService = new CustomerAppService(_unitOfWork, _customerRepository, _parameterRepository, _loggerCustomerAppService.Object);
        var _mapper = new MapperConfiguration(cfg => cfg.AddProfile<ConfiguringMapperProfile>()).CreateMapper();

        var _service = new CustomerCreatedService(_customerAppService, _logger.Object, _mapper);

        return new CustomerCreatedConsumer(_service);
    }
}