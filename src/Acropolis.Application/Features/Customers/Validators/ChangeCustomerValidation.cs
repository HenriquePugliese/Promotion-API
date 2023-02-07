using Acropolis.Application.Features.Customers.Requests;

namespace Acropolis.Application.Features.Customers.Validators;

public class ChangeCustomerValidation : CustomerValidation<ChangeCustomerRequest>
{
    public ChangeCustomerValidation()
    {
        ValidateCnpj();
        ValidateParameters();
    }
}
