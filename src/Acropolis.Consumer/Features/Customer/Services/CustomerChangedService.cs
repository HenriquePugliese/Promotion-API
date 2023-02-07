using Acropolis.Application.Features.Customers.Requests;
using Acropolis.Application.Features.Customers.Services.Contracts;
using Acropolis.Application.Features.Customers.Validators;
using Acropolis.Consumer.Features.Customer.Messages;
using AutoMapper;
using DotNetCore.CAP;
using Newtonsoft.Json;
using Ziggurat;

namespace Acropolis.Consumer.Features.Customer.Services;

public class CustomerChangedService : IConsumerService<CustomerChangedMessage>
{
    private readonly ICustomerAppService _customerAppService;
    private readonly ILogger<CustomerChangedService> _logger;
    private readonly IMapper _mapper;

    public CustomerChangedService(
        ICustomerAppService customerAppService,
        ILogger<CustomerChangedService> logger,
        IMapper mapper)
    {
        _customerAppService = customerAppService;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task ProcessMessageAsync(CustomerChangedMessage message)
    {
        _logger.LogInformation("Got {message}", nameof(CustomerChangedMessage));

        var validator = new ChangeCustomerValidation();
        var validatorResult = await validator.ValidateAsync(message);

        if (validatorResult.IsValid)
        {
            var (response, changedCustomer) = await _customerAppService.ChangeCustomerParameters(_mapper.Map<ChangeCustomerRequest>(message));

            if (!response.IsValid() || changedCustomer is null)
                throw new SubscriberNotFoundException(string.Join(Environment.NewLine, response.Notifications.Select(e => e.Description)));

            return;
        }

        _logger.LogError("Message validation error: {message}.\n Errors: {errors}",
            nameof(CustomerChangedMessage),
            JsonConvert.SerializeObject(validatorResult
                .Errors
                .Select(x => x.ErrorMessage)
                .ToArray()).ToString());
    }
}