using Acropolis.Application.Features.Promotions.Requests;
using FluentValidation;

namespace Acropolis.Application.Features.Promotions.Validators;

public class RemovePromotionValidator : AbstractValidator<RemovePromotionRequest>
{
    public RemovePromotionValidator()
    {
        RuleFor(p => p.ExternalId).NotEmpty().WithMessage($"Identificador externo da promoção é obrigatório.");
    }
}