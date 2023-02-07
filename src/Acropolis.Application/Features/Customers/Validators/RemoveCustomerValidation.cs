using Acropolis.Application.Features.Customers.Requests;

namespace Acropolis.Application.Features.Customers.Validators;

public class RemoveCustomerValidation : CustomerValidation<RemoveCustomerRequest>
{
    public RemoveCustomerValidation()
    {
        ValidateCnpj();
    }
}

