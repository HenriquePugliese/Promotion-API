using Acropolis.Consumer;
using Acropolis.Consumer.Extensions;
using Microsoft.Extensions.Hosting.Internal;
using Serilog;

IHostEnvironment _env = new HostingEnvironment();

var solutionSettings = Path.Combine(Directory.GetCurrentDirectory(), "..", "appsettings.json");

var configuration = new ConfigurationBuilder()
            .AddJsonFile(solutionSettings, optional: true, reloadOnChange: false)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
            .AddEnvironmentVariables()
            .Build();

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((host, _) =>
    {
        _env = host.HostingEnvironment;
    })
    .ConfigureServices(services =>
    {
        if (args is null || !args.Any())
            throw new ArgumentNullException(nameof(args));

        services.RegisterConsumerServiceResolver(args[0]);

        services.RegisterCapServicesResolver(configuration);

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();

        services.AddAutoMapperSetup();
        services.AddHealthChecksSetup(configuration);
        services.AddApplication(configuration);
        services.AddPersistence(configuration, _env);
        services.AddHostedService<Worker>();
    }).UseSerilog().Build();

await host.RunAsync();