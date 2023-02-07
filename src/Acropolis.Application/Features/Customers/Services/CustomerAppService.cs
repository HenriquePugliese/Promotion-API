using Acropolis.Application.Base.Models;
using Acropolis.Application.Base.Persistence;
using Acropolis.Application.Base.Services;
using Acropolis.Application.Features.Customers.Repositories;
using Acropolis.Application.Features.Customers.Requests;
using Acropolis.Application.Features.Customers.Services.Contracts;
using Acropolis.Application.Features.Parameters.Repositories;
using Microsoft.Extensions.Logging;
using Parameter = Acropolis.Application.Features.Parameters.Parameter;

namespace Acropolis.Application.Features.Customers.Services;

public class CustomerAppService : AppService, ICustomerAppService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IParameterRepository _parameterRepository;
    private readonly ILogger<CustomerAppService> _logger;

    public CustomerAppService(
        IUnitOfWork unitOfWork,
        ICustomerRepository customerRepository,
        IParameterRepository parameterRepository,
        ILogger<CustomerAppService> logger)
        : base(unitOfWork)
    {
        _customerRepository = customerRepository;
        _parameterRepository = parameterRepository;
        _logger = logger;
    }

    public async Task<(Response, Customer?)> CreateCustomer(CreateCustomerRequest request, CancellationToken cancellationToken = default)
    {
        var baseLogMessage = $"Create new customer ({request.Cnpj})";
        _logger.LogInformation("{baseLogMessage} - Started", baseLogMessage);

        var customer = new Customer(request.Cnpj, request.SellerId, request.CustomerCode);

        if (request.Parameters?.Any() == true)
            customer.Parameters = request.Parameters
                .Select(x => new Parameter(x.Description, x.Code, x.Value, x.Status, customer.Id))
                .ToList();

        try
        {
            await _customerRepository.Add(customer);

            await Commit();

            _logger.LogInformation("{baseLogMessage} - Success", baseLogMessage);

            return (Response.Valid(), customer);
        }
        catch (Exception exc)
        {
            _logger.LogError(exc, "{baseLogMessage} - Failure", baseLogMessage);

            return (Response.Invalid("Customer", exc.Message), customer);
        }
    }

    public async Task<Response> RemoveCustomer(RemoveCustomerRequest request, CancellationToken cancellationToken = default)
    {
        var baseLogMessage = $"Remove customer ({request.Cnpj})";
        _logger.LogInformation("{baseLogMessage} - Started", baseLogMessage);

        try
        {
            var customer = await _customerRepository.FindBy(x => x.Cnpj == request.Cnpj && x.SellerId == request.SellerId);

            if (customer is null)
                return Response.Invalid("Customer", "Customer not found");

            _customerRepository.Remove(customer);

            await Commit();

            _logger.LogInformation("{baseLogMessage} - Success", baseLogMessage);

            return Response.Valid();
        }
        catch (Exception exc)
        {
            _logger.LogError(exc, "{baseLogMessage} - Failure", baseLogMessage);

            return Response.Invalid("Customer", exc.Message);
        }
    }

    public async Task<(Response, Customer?)> ChangeCustomerParameters(ChangeCustomerRequest request, CancellationToken cancellationToken = default)
    {
        var baseLogMessage = $"Update customer ({request.Cnpj})";
        _logger.LogInformation("{baseLogMessage} - Started", baseLogMessage);

        var customer = await _customerRepository.FindBy(x => x.Cnpj == request.Cnpj && x.SellerId == request.SellerId);

        if (customer is null)
        {
            var createCustomerRequest = new CreateCustomerRequest()
            {
                Cnpj = request.Cnpj,
                CustomerCode = request.CustomerCode,
                SellerId = request.SellerId,
                Parameters = request.Parameters
            };

            return await CreateCustomer(createCustomerRequest, cancellationToken);
        }

        try
        {
            if (customer.Parameters?.Any() == true && request.Parameters?.Any() == true)
            {
                HandleCustomerParameters(request, customer, out bool isCountDifferent, out bool hasSameValues);

                if (isCountDifferent || !hasSameValues)
                {
                    _parameterRepository.RemoveRange(customer.Parameters);

                    _parameterRepository.AddRange(request.Parameters
                        .Select(x => new Parameter(x.Description, x.Code, x.Value, x.Status, customer.Id))
                        .ToList());

                    await Commit();
                }
            }

            _logger.LogInformation("{baseLogMessage} - Success", baseLogMessage);

            return (Response.Valid(), customer);
        }
        catch (Exception exc)
        {
            _logger.LogError(exc, "{baseLogMessage} - Failure", baseLogMessage);

            return (Response.Invalid("Customer", exc.Message), customer);
        }
    }

    private static void HandleCustomerParameters(ChangeCustomerRequest request, Customer customer, out bool isCountDifferent, out bool hasSameValues)
    {
        isCountDifferent = customer.Parameters?.Count != request.Parameters?.Count;

        if (customer.Parameters is null || request.Parameters is null)
        {
            hasSameValues = false;
            return;
        }

        hasSameValues = !customer.Parameters
            .Select(s => (s.Code, s.Value, s.Description, s.Status))
            .Except(request.Parameters.Select(s => (s.Code, s.Value, s.Description, s.Status)))
            .Any() && !isCountDifferent;
    }
}