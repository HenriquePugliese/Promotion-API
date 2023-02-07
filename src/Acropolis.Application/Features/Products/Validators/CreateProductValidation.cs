using Acropolis.Application.Features.Products.Requests;

namespace Acropolis.Application.Features.Products.Validators;

public class CreateProductValidation : ProductValidation<CreateProductRequest>
{
    public CreateProductValidation()
    {
        ValidateId();
        ValidateName();
        ValidateMaterialCode();
        ValidateUnitMeasure();
        ValidateWeight();
        ValidateUnitWeight();
    }
}

