using Ziggurat.CapAdapter;

namespace Acropolis.Consumer.Extensions;

public static class RegisterCapServicesExtensions
{
    public static void RegisterCapServicesResolver(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCap(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("SqlServer"));
            options.UseRabbitMQ(config =>
            {
                config.HostName = configuration["RabbitMQ:Host"];
                config.UserName = configuration["RabbitMQ:UserName"];
                config.Password = configuration["RabbitMQ:Password"];
                config.VirtualHost = configuration["RabbitMQ:VirtualHost"];
                config.Port = configuration.GetValue<int>("RabbitMQ:Port");
                config.ExchangeName = configuration["RabbitMQ:ExchangeName"];
            });
        }).AddSubscribeFilter<BootstrapFilter>();
    }
}