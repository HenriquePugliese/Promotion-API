using Acropolis.Application.Base.Persistence;
using Acropolis.Application.Features.Attributes.Repositories;
using Acropolis.Application.Features.Customers.Repositories;
using Acropolis.Application.Features.Parameters.Repositories;
using Acropolis.Application.Features.Products.Repositories;
using Acropolis.Application.Features.Promotions.Repositories;
using Acropolis.Infrastructure.Contexts;
using Acropolis.Infrastructure.Contexts.Persistence;
using Acropolis.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Acropolis.Api.Extensions;

public static class PersistenceExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration, IHostEnvironment env)
    {
        // contexts

        if (!env.IsEnvironment("Testing"))
        {
            services.AddDbContext<AcropolisContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("SqlServer"), builder =>
                    builder.MigrationsAssembly("Acropolis.Infrastructure"))
                );
        }

        // add unit of work

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // repositories

        services.AddTransient<IAttributeRepository, AttributeRepository>();
        services.AddTransient<IPromotionRepository, PromotionRepository>();
        services.AddTransient<IPromotionCnpjRepository, PromotionCnpjRepository>();
        services.AddTransient<IPromotionCnpjDiscountLimitRepository, PromotionCnpjDiscountLimitRepository>();
        services.AddTransient<IProductRepository, ProductRepository>();
        services.AddTransient<ICustomerRepository, CustomerRepository>();
        services.AddTransient<IParameterRepository, ParameterRepository>();

        return services;
    }
}