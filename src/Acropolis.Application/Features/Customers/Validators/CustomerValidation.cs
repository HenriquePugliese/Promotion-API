using Acropolis.Application.Features.Customers.Requests;
using Acropolis.Application.Features.Parameters.Validators;
using FluentValidation;

namespace Acropolis.Application.Features.Customers.Validators;

public class CustomerValidation<T> : AbstractValidator<T> where T : CustomerRequest
{
    protected void ValidateCnpj()
    {
        RuleFor(c => c.Cnpj)
       .NotNull().WithMessage("O CNPJ do cliente é obrigatório")
       .NotEmpty().WithMessage("A CNPJ do cliente é obrigatório");
    }

    protected void ValidateParameters()
    {
        When(c => c.Parameters?.Any() == true, () =>
        {
            RuleForEach(c => c.Parameters).SetValidator(new ParameterValidator());
        }).Otherwise(() =>
        {
            RuleForEach(c => c.Parameters)
            .NotNull()
            .WithMessage("Os parâmetros do cliente são obrigatórios");
        });
    }
}
