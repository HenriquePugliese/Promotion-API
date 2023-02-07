using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace Acropolis.Tests.Helpers
{
    public class TestingWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        private readonly Action<IServiceCollection> _configureTestServices;

        /// <summary>
        /// Creates a new instance of a JSM.Crosscutting.Common.TestSupport.TestingWebApplicationFactory
        /// that can be used in test fixtures to build fully DI configured apps.
        /// </summary>
        /// <param name="configureTestServices">
        /// A System.Action of Microsoft.Extensions.DependencyInjection.IServiceCollection to be executed on the
        /// ConfigureTestServices step of the Host creation. This callback is executed after TStartup.ConfigureServices
        /// and can be used to inject mocked services and "override" TStartup services.
        /// </param>
        public TestingWebApplicationFactory(Action<IServiceCollection>? configureTestServices = null)
        {
            _configureTestServices = configureTestServices ?? (services => { });
        }

        /// <summary>
        /// Creates a new Microsoft.AspNetCore.Hosting.IHostBuilder used to set up a Microsoft.AspNetCore.TestHost.TestServer
        /// which contains the DI setup given by the TStartup class andor the configureTestServices action.
        /// </summary>
        /// <returns>A Microsoft.AspNetCore.Hosting.IHostBuilder ASP.NET Core 5.0 instance.</returns>
        protected override IHostBuilder CreateHostBuilder()
        {
            return CreateDefaultBuilderNoReloadOnChange()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<TStartup>();
                    webBuilder.UseEnvironment("Testing");
                    webBuilder.ConfigureTestServices(_configureTestServices);
                }).UseSerilog();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IHostBuilder"/> class with pre-configured defaults. As this builder
        /// is designed for testing, the json configuration providers are used without requesting file watchers from the host OS
        /// (such as inotifiers on Linux) so that web hosts can be built without exceeding inotify limits.
        /// 
        /// The IConfigurationBuilder used by this IHostBuilder is configured to look for settings jsons inside the 
        /// test project's bin folder (ex: JSM.Customer.Test/bin/Debug/netcore5.0) and not inside the target TStartup project's 
        /// folder (ex: JSM.Customer.Api) as some apps, such as Customer, have no settings jsons inside the Api project's folder but 
        /// have them placed inside the bin folder after compilation.
        /// </summary>
        /// <param name="args">The command line args</param>
        /// <returns>The initialized  <see cref="IHostBuilder"/> instance</returns>
        private static IHostBuilder CreateDefaultBuilderNoReloadOnChange(string[]? args = null)
        {
            // current working directory -> bin folders with the compiled assemblies 
            // ex: JSM.Customer.Test/bin/Debug/netcoreapp2.2
            var binOutputFolder = Directory.GetCurrentDirectory();

            var builder = new HostBuilder()
                .UseContentRoot(binOutputFolder)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var env = hostingContext.HostingEnvironment;

                    // reloadOnChange is false so no OS file-monitoring API, e.g. inotify API on Linux, is used
                    config
                        .SetBasePath(binOutputFolder)  // lookup for settings jsons inside bin folder and NOT TStartup's project folder
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: false);

                    if (env.IsDevelopment())
                    {
                        var appAssembly = Assembly.Load(new AssemblyName(env.ApplicationName));
                        if (appAssembly != null)
                        {
                            config.AddUserSecrets(appAssembly, optional: true);
                        }
                    }

                    config.AddEnvironmentVariables();

                    if (args != null)
                    {
                        config.AddCommandLine(args);
                    }
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                    logging.AddDebug();
                })
                .UseDefaultServiceProvider((context, options) =>
                {
                    options.ValidateScopes = context.HostingEnvironment.IsDevelopment();
                });

            if (args != null)
            {
                builder.ConfigureAppConfiguration(builder => builder.AddConfiguration(new ConfigurationBuilder().AddCommandLine(args).Build()));
            }

            return builder;
        }

    }
}
