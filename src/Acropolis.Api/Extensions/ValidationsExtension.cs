using Acropolis.Application.Features.Products.Validators;
using Acropolis.Application.Features.Promotions.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;

namespace Acropolis.Api.Extensions;

public static class ValidationsExtension
{
    public static IServiceCollection AddValidations(this IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation();
        
        services.AddValidatorsFromAssemblyContaining<CreatePromotionValidator>();

        services.AddValidatorsFromAssemblyContaining<CreatePromotionCnpjValidator>();

        services.AddValidatorsFromAssemblyContaining<CreateProductValidation>();

        services.AddValidatorsFromAssemblyContaining<RemovePromotionValidator>();

        services.AddValidatorsFromAssemblyContaining<GetPromotionOrderValidator>();

        return services;
    }
}
