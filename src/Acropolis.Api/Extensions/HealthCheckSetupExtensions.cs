using Acropolis.Infrastructure.Contexts;

namespace Acropolis.Api.Extensions;

public static class HealthCheckSetupExtensions
{
    public static IServiceCollection AddHealthChecksSetup(this IServiceCollection services, IConfiguration configuration)
    {
        // Add dependencies health checks here

        services.AddHealthChecks()
            .AddDbContextCheck<AcropolisContext>(name: "database")
            .AddSqlServer(configuration.GetConnectionString("SqlServer"));

        return services;
    }
}
