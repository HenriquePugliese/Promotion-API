using Acropolis.Application.Features.Promotions.Requests;
using FluentValidation;

namespace Acropolis.Application.Features.Promotions.Validators;

public class GetPromotionOrderValidator : AbstractValidator<PromotionOrderRequest>
{
    public GetPromotionOrderValidator()
    {
        RuleFor(promotionOrderRequest => promotionOrderRequest.CustomerCode)
            .Must(customerCode => !string.IsNullOrWhiteSpace(customerCode))
            .WithMessage(BuildRequiredMessageFor("CustomerCode"));

        RuleFor(promotionOrderRequest => promotionOrderRequest.ProductItems)
            .Must(items => items is not null && items.Any() && !items.Any(item => item.ProductId == Guid.Empty))
            .WithMessage(BuildRequiredMessageFor("ProductItems"));

        RuleFor(promotionOrderRequest => promotionOrderRequest.SellerId)
            .Must(sellerId => sellerId != Guid.Empty)
            .WithMessage(BuildRequiredMessageFor("SellerId"));
    }

    private static string BuildRequiredMessageFor(string label) => $"Campo '{label}' é obrigatório.";
}