using Acropolis.Application.Features.Promotions.Requests;
using FluentValidation;

namespace Acropolis.Application.Features.Promotions.Validators;

public class CreatePromotionCnpjValidator : AbstractValidator<CreatePromotionCnpjRequest>
{
    public CreatePromotionCnpjValidator()
    {
        const int cnpjsMinAmount = 1;
        const int cnpjsMaxAmount = 1000;

        RuleFor(promotionCnpjRequest => promotionCnpjRequest.ExternalId)
            .NotEmpty().WithMessage($"Identificador externo da promoção é obrigatório.");

        RuleFor(promotionRequest => promotionRequest.Cnpjs)
            .NotEmpty().WithMessage($"No mínimo {cnpjsMinAmount} CNPJ é obrigatório.")
            .Must(cnpjs => cnpjs.Count() <= cnpjsMaxAmount).WithMessage($"No máximo {cnpjsMaxAmount} CNPJs são permitidos.");

        RuleForEach(promotionRequest => promotionRequest.Cnpjs).ChildRules(cnpj =>
        {
            cnpj.RuleFor(cnpj => cnpj)
                .NotEmpty().WithMessage($"CNPJ é obrigatório.")
                .Must(cnpj => CnpjValidator.Validate(cnpj)).WithMessage((_,cnpj) => $"CNPJ '{cnpj}' é inválido.");
        });
    }
}