using Acropolis.Application.Features.Attributes.Services;
using Acropolis.Application.Features.Attributes.Services.Contracts;
using Acropolis.Application.Features.Customers.Services;
using Acropolis.Application.Features.Customers.Services.Contracts;
using Acropolis.Application.Features.Products.Services;
using Acropolis.Application.Features.Products.Services.Contracts;

namespace Acropolis.Consumer.Extensions;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        // Add handlers, services, repositories

        services.AddTransient<IAttributeAppService, AttributeAppService>();
        services.AddTransient<IProductAppService, ProductAppService>();
        services.AddTransient<ICustomerAppService, CustomerAppService>();

        return services;
    }
}
