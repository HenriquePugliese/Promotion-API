using Acropolis.Application.Features.Parameters.Requests;
using FluentValidation;

namespace Acropolis.Application.Features.Parameters.Validators;

public class ParameterValidator: AbstractValidator<ParameterRequest>
{
    public ParameterValidator()
    {
        RuleFor(c => c.Code)
            .NotEmpty().WithMessage("O código do parâmetro é obrigatório")
            .NotNull().WithMessage("O código do parâmetro é obrigatório");

        RuleFor(c => c.Value)
            .NotEmpty().WithMessage("O valor do parâmetro é obrigatório")
            .NotNull().WithMessage("O valor do parâmetro é obrigatório");
    }
}
