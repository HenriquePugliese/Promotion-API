using Acropolis.Application.Features.Promotions.Requests;
using FluentValidation;
using System.Linq.Expressions;

namespace Acropolis.Application.Features.Promotions.Validators;

public class CreatePromotionValidator : AbstractValidator<CreatePromotionRequest>
{
    public CreatePromotionValidator()
    {
        RuleRequiredFor(promotionRequest => promotionRequest.Name, "Nome");
        RuleRequiredFor(promotionRequest => promotionRequest.DtStart, "Data inicial");
        RuleRequiredFor(promotionRequest => promotionRequest.DtEnd, "Data final");
        RuleRequiredFor(promotionRequest => promotionRequest.UnitMeasurement, "A unidade de medida");
        RuleRequiredFor(promotionRequest => promotionRequest.Rules, "Ao menos uma regra");
        RuleRequiredFor(promotionRequest => promotionRequest.Attributes, "Ao menos um atributo");

        RuleFor(promotionRequest => promotionRequest.DtEnd)
                .Must(dtEnd => dtEnd >= DateTime.Today).WithMessage("A data final da promoção não pode ser menor do que a data atual.");

        RuleForEach(promotionRequest => promotionRequest.Attributes)
                .Must(attr => Guid.TryParse(attr.AttributesId, out Guid attrId)).WithMessage("Valor do campo `AttributesId` inválido.");

        RuleLengthForName();
    }

    private void RuleLengthForName()
    {
        const int promotionNameMinLength = 3;
        const int promotionNameMaxLength = 100;
        const string promotionNameLengthWarning = "Nome da promoção deve conter no {0} {1} caracteres.";

        RuleFor(promotionRequest => promotionRequest.Name)
            .MinimumLength(promotionNameMinLength).WithMessage(String.Format(promotionNameLengthWarning, "mínimo", promotionNameMinLength))
            .MaximumLength(promotionNameMaxLength).WithMessage(string.Format(promotionNameLengthWarning, "máximo", promotionNameMaxLength));
    }

    private void RuleRequiredFor<TProperty>(Expression<Func<CreatePromotionRequest, TProperty>> expression, string label)
    {
        RuleFor(expression)
            .NotEmpty().WithMessage($"{label} da promoção é obrigatório.");
    }
}