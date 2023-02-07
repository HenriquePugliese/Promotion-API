using Acropolis.Application.Features.Attributes.Requests;

namespace Acropolis.Application.Features.Attributes.Validators;

public class CreateAttributeValidation : AttributeValidation<CreateAttributeRequest>
{
    public CreateAttributeValidation()
    {
        ValidateProductId();
        ValidateAttributeKeyId();
        ValidateAttributeKey();
        ValidateAttributeKeyLabel();
        ValidateAttributeValueId();
        ValidateAttributeValue();
        ValidateAttributeValueLabel();
    }
}

