using Acropolis.Api;
using Serilog;

public class Program
{
    protected Program()
    { }

    public static void Main(string[] args)
    {
        ConfigureLogger();

        try
        {
            CreateHostBuilder(args).Build().Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "WebHost server has terminated unexpectedly!");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    private static void ConfigureLogger()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        var builtConfig = configuration.AddEnvironmentVariables().Build();

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builtConfig)
            .CreateLogger();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            }).UseSerilog();
}