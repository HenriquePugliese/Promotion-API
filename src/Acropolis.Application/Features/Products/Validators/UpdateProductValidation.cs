using Acropolis.Application.Features.Products.Requests;
using FluentValidation;

namespace Acropolis.Application.Features.Products.Validators;

public class UpdateProductValidation : ProductValidation<UpdateProductRequest>
{
    public UpdateProductValidation()
    {
        ValidateId();
        ValidateName();
        ValidateMaterialCode();
        ValidateUnitMeasure();
        ValidateWeight();
        ValidateUnitWeight();
    }
}
