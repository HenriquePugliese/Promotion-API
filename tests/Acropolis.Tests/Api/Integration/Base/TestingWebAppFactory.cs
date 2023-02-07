using Acropolis.Infrastructure.Contexts;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Acropolis.Tests.Api.Integration.Base;

public class TestingWebAppFactory<TEntryPoint> : WebApplicationFactory<Program> where TEntryPoint : Program
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                    typeof(DbContextOptions<AcropolisContext>));

            if (descriptor != null)
                services.Remove(descriptor);

            services.AddDbContext<AcropolisContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryAcropolisTest");
            });

            AssertionOptions.FormattingOptions.MaxLines = 5000;

            var sp = services.BuildServiceProvider();
            using (var scope = sp.CreateScope())
            using (var appContext = scope.ServiceProvider.GetRequiredService<AcropolisContext>())
            {
                appContext.Database.EnsureCreated();
                var databaseSeed = new AcropolisDatabaseSeed(appContext);

                databaseSeed.SeedData();
            }
        });
    }
}