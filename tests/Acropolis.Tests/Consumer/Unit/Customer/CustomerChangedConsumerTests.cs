using Acropolis.Application.Features.Customers.Services;
using Acropolis.Application.Features.Customers.Validators;
using Acropolis.Application.Features.Parameters.Requests;
using Acropolis.Consumer.Common;
using Acropolis.Consumer.Common.AutoMapper;
using Acropolis.Consumer.Features.Customer.Consumer;
using Acropolis.Consumer.Features.Customer.Messages;
using Acropolis.Consumer.Features.Customer.Services;
using Acropolis.Infrastructure.Contexts;
using Acropolis.Infrastructure.Contexts.Persistence;
using Acropolis.Infrastructure.Repositories;
using Acropolis.Tests.Consumer.Support.Data;
using Acropolis.Tests.Helpers;
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

public class CustomerChangedConsumerTests : TestProgram<Program>
{
    private readonly Mock<ILogger<CustomerChangedService>> _logger;
    private readonly CustomerChangedConsumer _consumer;
    private readonly AcropolisContext _context;

    public CustomerChangedConsumerTests()
    {
        _logger = new Mock<ILogger<CustomerChangedService>>();
        _context = Server.Services.GetRequiredService<AcropolisContext>();
        _consumer = BuildService();
    }

    [Fact(DisplayName = "Should Customer ChangedSubscription with success")]
    public async Task Should_ChangedSubscription_WithSuccess()
    {
        // Arrange
        var customer = await SqlTestData.CustomerSeeder(Server.Services.CreateScope());

        var message = new CustomerChangedMessage(Guid.NewGuid().ToString(), RouterKeys.GroupCustomerChanged)
        {
            Cnpj = customer.Cnpj,
            SellerId = customer.SellerId,
            Parameters = new List<ParameterRequest>() {
                new()
                {
                    Description = "VC-SP-ITAPETININGA",
                    Code = "Mesoregiao" ,
                    Value = "35B" ,
                    Status = false
                },
                new()
                {
                    Description = "VC-DISTRIBUIDOR" ,
                    Code = "Microregiao" ,
                    Value = "3B8" ,
                    Status = true
                }
            }
        };

        // Act
        Func<Task> act = async () => await _consumer.CustomerChangedSubscription(message);

        // Assert
        await act.Should()
            .NotThrowAsync();

        var customerDb = _context.Customers.FirstOrDefault(x => x.Cnpj == message.Cnpj && x.SellerId == message.SellerId);

        customerDb.Should().NotBeNull();

        // Assert Log
        _logger.VerifyLogging("", LogLevel.Error, Times.Never());
    }

    [Fact(DisplayName = "Should Customer ChangedSubscription with validation error")]
    public async Task Should_ChangedSubscription_WithValidationError()
    {
        // Arrange
        var message = new CustomerChangedMessage(Guid.NewGuid().ToString(), RouterKeys.GroupCustomerChanged)
        {
            Cnpj = string.Empty,
            SellerId = string.Empty,
            Parameters = new List<ParameterRequest>() {
                new()
                {
                    Description = "VC-SP-ITAPETININGA",
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

        var validator = new ChangeCustomerValidation();
        var validatorResult = await validator.ValidateAsync(message);
        var loggerMessage = string.Format("Message validation error: {0}.\n Errors: {1}",
            nameof(CustomerChangedMessage),
            JsonConvert.SerializeObject(validatorResult
                .Errors
                .Select(x => x.ErrorMessage)
                .ToArray()));

        // Act
        Func<Task> act = async () => await _consumer.CustomerChangedSubscription(message);

        // Assert
        await act.Should()
            .NotThrowAsync();

        _logger.VerifyLogging(loggerMessage, LogLevel.Error);
    }

    [Fact(DisplayName = "Should Customer ChangedSubscription with error")]
    public async Task Should_ChangedSubscription_WithError()
    {
        // Arrange
        var message = new CustomerChangedMessage(Guid.NewGuid().ToString(), RouterKeys.GroupCustomerChanged)
        {
            Cnpj = new Faker("pt_BR").Company.Cnpj(),
            Parameters = new List<ParameterRequest>() {
                new()
                {
                    Description = "VC-SP-ITAPETININGA",
                    Code = "Mesoregiao" ,
                    Value = "35B" ,
                    Status = false
                },
                new()
                {
                    Description = "VC-DISTRIBUIDOR" ,
                    Code = "Microregiao" ,
                    Value = "3B8" ,
                    Status = true
                }
            }
        };

        // Act
        Func<Task> act = async () => await _consumer.CustomerChangedSubscription(message);

        // Assert
        await act.Should()
            .ThrowExactlyAsync<SubscriberNotFoundException>();
    }

    [Fact(DisplayName = "Should Customer ChangedSubscription with success when customer not exists")]
    public async Task Should_ChangedSubscription_WithSuccessWhenCustomerNotExists()
    {
        // Arrange
        var message = new CustomerChangedMessage(Guid.NewGuid().ToString(), RouterKeys.GroupCustomerChanged)
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
        Func<Task> act = async () => await _consumer.CustomerChangedSubscription(message);

        // Assert
        await act.Should()
            .NotThrowAsync();

        var customerDb = _context.Customers.FirstOrDefault(x => x.Cnpj == message.Cnpj && x.SellerId == message.SellerId);

        customerDb.Should().NotBeNull();

        // Assert Log
        _logger.VerifyLogging("", LogLevel.Error, Times.Never());
    }

    private CustomerChangedConsumer BuildService()
    {
        var _loggerCustomerAppService = new Mock<ILogger<CustomerAppService>>();

        var _unitOfWork = new UnitOfWork(_context);
        var _customerRepository = new CustomerRepository(_context);
        var _parameterRepository = new ParameterRepository(_context);
        var _customerAppService = new CustomerAppService(_unitOfWork, _customerRepository, _parameterRepository, _loggerCustomerAppService.Object);
        var _mapper = new MapperConfiguration(cfg => cfg.AddProfile<ConfiguringMapperProfile>()).CreateMapper();

        var _service = new CustomerChangedService(_customerAppService, _logger.Object, _mapper);

        return new CustomerChangedConsumer(_service);
    }
}