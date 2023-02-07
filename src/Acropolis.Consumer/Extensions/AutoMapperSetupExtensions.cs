using Acropolis.Consumer.Common.AutoMapper;

namespace Acropolis.Consumer.Extensions;

public static class AutoMapperSetupExtensions
{
    public static void AddAutoMapperSetup(this IServiceCollection services)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        services.AddAutoMapper(typeof(ConfiguringMapperProfile));
    }
}