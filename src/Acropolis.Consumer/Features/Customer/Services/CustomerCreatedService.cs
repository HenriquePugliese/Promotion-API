using Acropolis.Application.Features.Customers.Requests;
using Acropolis.Application.Features.Customers.Services.Contracts;
using Acropolis.Application.Features.Customers.Validators;
using Acropolis.Consumer.Features.Customer.Messages;
using AutoMapper;
using DotNetCore.CAP;
using Newtonsoft.Json;
using Ziggurat;

namespace Acropolis.Consumer.Features.Customer.Services;

public class CustomerCreatedService : IConsumerService<CustomerCreatedMessage>
{
    private readonly ICustomerAppService _customerAppService;
    private readonly ILogger<CustomerCreatedService> _logger;
    private readonly IMapper _mapper;

    public CustomerCreatedService(
        ICustomerAppService customerAppService,
        ILogger<CustomerCreatedService> logger,
        IMapper mapper)
    {
        _customerAppService = customerAppService;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task ProcessMessageAsync(CustomerCreatedMessage message)
    {
        _logger.LogInformation("Got {message}", nameof(CustomerCreatedMessage));

        var validator = new CreateCustomerValidation();
        var validatorResult = await validator.ValidateAsync(message);

        if (validatorResult.IsValid)
        {
            var (response, createdCustomer) = await _customerAppService.CreateCustomer(_mapper.Map<CreateCustomerRequest>(message));

            if (!response.IsValid() || createdCustomer is null)
                throw new SubscriberNotFoundException(string.Join(Environment.NewLine, response.Notifications.Select(d => d.Description)));

            return;
        }

        _logger.LogError("Message validation error: {message}.\n Errors: {errors}",
            nameof(CustomerCreatedMessage),
            JsonConvert.SerializeObject(validatorResult
                .Errors
                .Select(x => x.ErrorMessage)
                .ToArray()).ToString());
    }
}