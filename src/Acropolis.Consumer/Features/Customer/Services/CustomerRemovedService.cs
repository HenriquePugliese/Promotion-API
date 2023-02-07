using Acropolis.Application.Features.Customers.Requests;
using Acropolis.Application.Features.Customers.Services.Contracts;
using Acropolis.Application.Features.Customers.Validators;
using Acropolis.Consumer.Features.Customer.Messages;
using AutoMapper;
using DotNetCore.CAP;
using Newtonsoft.Json;
using Ziggurat;

namespace Acropolis.Consumer.Features.Customer.Services;

public class CustomerRemovedService : IConsumerService<CustomerRemovedMessage>
{
    private readonly ICustomerAppService _customerAppService;
    private readonly ILogger<CustomerRemovedService> _logger;
    private readonly IMapper _mapper;

    public CustomerRemovedService(
        ICustomerAppService customerAppService,
        ILogger<CustomerRemovedService> logger,
        IMapper mapper)
    {
        _customerAppService = customerAppService;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task ProcessMessageAsync(CustomerRemovedMessage message)
    {
        _logger.LogInformation("Got {message}", nameof(CustomerRemovedMessage));

        var validator = new RemoveCustomerValidation();
        var validatorResult = await validator.ValidateAsync(message);

        if (validatorResult.IsValid)
        {
            var response = await _customerAppService.RemoveCustomer(_mapper.Map<RemoveCustomerRequest>(message));

            if (!response.IsValid())
                throw new SubscriberNotFoundException(string.Join(Environment.NewLine, response.Notifications.Select(e => e.Description)));

            return;
        }

        _logger.LogError("Message validation error: {message}.\n Errors: {errors}",
            nameof(CustomerRemovedMessage),
            JsonConvert.SerializeObject(validatorResult
                .Errors
                .Select(x => x.ErrorMessage)
                .ToArray()).ToString());
    }
}