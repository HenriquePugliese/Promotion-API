using Acropolis.Application.Features.Products.Requests;
using FluentValidation;

namespace Acropolis.Application.Features.Products.Validators;

public class RemoveProductValidation : ProductValidation<RemoveProductRequest>
{
    public RemoveProductValidation()
    {
        ValidateId();
    }
}
