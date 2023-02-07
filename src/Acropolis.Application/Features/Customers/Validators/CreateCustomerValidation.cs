using Acropolis.Application.Features.Customers.Requests;

namespace Acropolis.Application.Features.Customers.Validators;

public class CreateCustomerValidation : CustomerValidation<CreateCustomerRequest>
{
    public CreateCustomerValidation()
    {
        ValidateCnpj();
        ValidateParameters();
    }
}
