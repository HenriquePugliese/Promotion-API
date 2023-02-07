using Acropolis.Infrastructure.Contexts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Acropolis.Tests.Helpers;

public class TestProgram<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");
        builder.UseEnvironment("Testing");
        builder.ConfigureServices(ConfigureTest);
        builder.ConfigureTestServices((services) =>
        {
            services.RemoveAll(typeof(IHostedService));
        });
    }

    public static void ConfigureTest(IServiceCollection services)
    {
        services.AddDbContext<AcropolisContext>(options =>
        {
            options.UseInMemoryDatabase("AcropolisTest");
            options.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));
        });
    }
}