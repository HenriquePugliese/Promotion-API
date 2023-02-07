using Acropolis.Application.Features.Attributes.Requests;
using FluentValidation;

namespace Acropolis.Application.Features.Attributes.Validators;

public class AttributeValidation<T> : AbstractValidator<T> where T : AttributeRequest
{
    protected void ValidateProductId()
    {
        RuleFor(c => c.ProductId)
            .NotEmpty().WithMessage("O identificador do produto não pode ser nulo");
    }

    protected void ValidateAttributeKeyId()
    {
        RuleFor(c => c.AttributeKeyId)
            .NotEmpty().WithMessage("O identificador do chave do atributo não pode ser nulo");
    }

    protected void ValidateAttributeKey()
    {
        RuleFor(c => c.AttributeKey)
            .NotNull().WithMessage("A chave do atributo não pode ser nulo")
            .NotEmpty().WithMessage("A chave do atributo não pode ser nulo");
    }

    protected void ValidateAttributeKeyLabel()
    {
        RuleFor(c => c.AttributeKeyLabel)
            .NotNull().WithMessage("O label da chave do atributo não pode ser nulo")
            .NotEmpty().WithMessage("O label da chave do atributo não pode ser nulo");
    }

    protected void ValidateAttributeValueId()
    {
        RuleFor(c => c.AttributeValueId)
            .NotEmpty().WithMessage("O identificado do valor do atributo não pode ser nulo");
    }

    protected void ValidateAttributeValue()
    {
        RuleFor(c => c.AttributeValue)
            .NotNull().WithMessage("O valor do atributo não pode ser nulo")
            .NotEmpty().WithMessage("O valor do atributo não pode ser nulo");
    }

    protected void ValidateAttributeValueLabel()
    {
        RuleFor(c => c.AttributeValueLabel)
            .NotNull().WithMessage("O label do valor do atributo não pode ser nulo")
            .NotEmpty().WithMessage("O label do valor do atributo não pode ser nulo");
    }
}

