using Acropolis.Application.Features.Attributes.Services;
using Acropolis.Application.Features.Attributes.Services.Contracts;
using Acropolis.Application.Features.Customers.Services;
using Acropolis.Application.Features.Customers.Services.Contracts;
using Acropolis.Application.Features.Products.Services;
using Acropolis.Application.Features.Products.Services.Contracts;
using Acropolis.Application.Features.Promotions.Services;
using Acropolis.Application.Features.Promotions.Services.Contracts;

namespace Acropolis.Api.Extensions;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        // Add handlers, services, repositories

        services.AddTransient<IAttributeAppService, AttributeAppService>();
        services.AddTransient<IPromotionAppService, PromotionAppService>();
        services.AddTransient<IPromotionProductAppService, PromotionProductAppService>();
        services.AddTransient<IPromotionDiscountAppService, PromotionOrderAppService>();
        services.AddTransient<IProductCardAppService, ProductCardAppService>();
        services.AddTransient<IDiscountLimitAppService, DiscountLimitAppService>();
        services.AddTransient<IProductAppService, ProductAppService>();
        services.AddTransient<ICustomerAppService, CustomerAppService>();

        return services;
    }
}
