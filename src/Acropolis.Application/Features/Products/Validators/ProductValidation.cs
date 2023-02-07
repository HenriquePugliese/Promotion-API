using Acropolis.Application.Features.Products.Requests;
using FluentValidation;

namespace Acropolis.Application.Features.Products.Validators;

public class ProductValidation<T> : AbstractValidator<T> where T : BaseProductRequest
{
    protected void ValidateId()
    {
        RuleFor(p => p.Id).NotEmpty();
    }
    protected void ValidateName()
    {
        RuleFor(p => p.Name)
            .NotEmpty().WithMessage("O campo [Name] não pode ser nulo ou vázio.")
            .MaximumLength(200).WithMessage("O campo [Name] deve conter no máximo 200 caracteres.");
    }

    protected void ValidateMaterialCode()
    {
        RuleFor(p => p.MaterialCode)
            .NotEmpty().WithMessage("O campo [MaterialCode] não pode ser nulo ou vázio.")
            .MaximumLength(40).WithMessage("O campo [MaterialCode] deve conter no máximo 40 caracteres.");
    }

    protected void ValidateUnitMeasure()
    {
        RuleFor(p => p.UnitMeasure)
            .NotEmpty().WithMessage("O campo [UnitMeasure] não pode ser nulo ou vázio.")
            .MaximumLength(3).WithMessage("O campo [UnitMeasure] deve conter no máximo 3 caracteres.");
    }

    protected void ValidateWeight()
    {
        RuleFor(p => p.Weight)
            .GreaterThanOrEqualTo(0)
            .WithMessage("O campo [Weight] não pode ser menor que zero.");
    }

    protected void ValidateUnitWeight()
    {
        RuleFor(p => p.UnitWeight)
            .MaximumLength(10).WithMessage("O campo [UnitWeight] deve conter no máximo 10 caracteres.");
    }
}
