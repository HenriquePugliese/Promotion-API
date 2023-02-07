using Microsoft.OpenApi.Models;

namespace Acropolis.Api.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Acropolis API", Version = "v1" });
        });

        return services;
    }
}
