using Acropolis.Application.Base.Persistence;
using Acropolis.Infrastructure.Contexts;
using Acropolis.Tests.Helpers.Api;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Moq;
using System.Data.SqlClient;
using System.Security.Claims;

namespace Acropolis.Tests.Helpers
{
    public class IntegrationTestBase<TStartup> where TStartup : class
    {
        protected AcropolisContext? TestSqlContext { get; private set; }
        protected IServiceProvider? ServiceProvider;
        protected Mock<HttpMessageHandler>? HttpMessageHandlerMock;
        protected IMapper? Mapper => ServiceProvider?.GetRequiredService<IMapper>();
        
        private TestingWebApplicationFactory<TStartup>? _webApplicationFactory;
        private HttpClient? TestAppClient;

        public IntegrationTestBase(bool configureServer = true)
        {
            if (configureServer)
                ConfigureServer().GetAwaiter().GetResult();
        }

        protected async Task ConfigureServer()
        {
            _webApplicationFactory = new TestingWebApplicationFactory<TStartup>(ConfigureServices);
            TestAppClient = _webApplicationFactory.CreateClient();

            // convenience variables used by the test classes
            ServiceProvider = _webApplicationFactory.Server.Services;

            // Change redis cache key for test isolation
            var config = ServiceProvider.GetRequiredService<IConfiguration>();
            config["CacheKeysPrefix"] = Guid.NewGuid().ToString();

            // hook after test server creation
            await AfterTestServerCreation(config);
        }

        protected HttpClient GetTestAppClient() => TestAppClient ?? new HttpClient();

        //
        //  Summary:
        //    Setups DI for the test application. In the future, this should be replacted by a TestStartUp class.
        //    Children classes should override this method for customized DI.
        //
        //    This method is invoke BEFORE the TStartup configuration class is run.
        protected virtual void ConfigureServices(IServiceCollection services)
        {
            // The same configuration used by the acropolis.API is used with ASPNETCORE_ENVIRONMENT=Testing
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .AddEnvironmentVariables()
                .Build();

            var connection = new SqlConnectionStringBuilder(configuration.GetConnectionString("SqlServer"))
            {
                PersistSecurityInfo = true,
            };

            // database for testing.
            services.AddDbContext<AcropolisContext>(options =>
            {
                //options.UseSqlServer(connection.ConnectionString);
                options.UseInMemoryDatabase("InMemoryAcropolisTest");
            });

            services.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                var config = new OpenIdConnectConfiguration()
                {
                    Issuer = JwtMock.Issuer
                };

                config.SigningKeys.Add(JwtMock.SecurityKey);
                options.Configuration = config;
            });

            AfterConfigureServices(services);
        }

        //
        //  Summary:
        //    Hook that is executed right after the default test DI services are configured. Should be inherited
        //    by classes that want to add extra steps without overriding the default DI configuration.
        protected virtual void AfterConfigureServices(IServiceCollection services)
        {
            HttpMessageHandlerMock = new Mock<HttpMessageHandler>();

            // mocked httpClientFactory with mocked messageHandler for mocking HTTP requests
            services.AddTransient(_ =>
            {
                var httpClientFactoryMock = new Mock<IHttpClientFactory>();
                httpClientFactoryMock
                    .Setup(httpFactory => httpFactory.CreateClient(It.IsAny<string>()))
                    .Returns((string clientName) => new HttpClient(HttpMessageHandlerMock.Object));
                return httpClientFactoryMock.Object;
            });

            var descriptorIow = services.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(IUnitOfWork));

            if (descriptorIow?.ImplementationInstance != null)
                services.Remove(descriptorIow);
        }

        //
        //  Summary:
        //    Hook that is executed after the test application was created. Should be inherited by classes that
        //    want to add some extra setup after the app creation such as avoiding the database seeding, adding extra
        //    context, etc.
        protected async virtual Task AfterTestServerCreation(IConfiguration configuration)
        {
            TestSqlContext = ServiceProvider?.GetRequiredService<AcropolisContext>();
            
            if (TestSqlContext is null)
                return;

            await TestSqlContext.Database.EnsureCreatedAsync();
            var databaseSeed = new AcropolisDatabaseSeed(TestSqlContext);
            databaseSeed.SeedData();
        }

        public void Dispose()
        {
            TestAppClient?.Dispose();
        }

        private static readonly LoggerFactory _loggerFactory
              = new LoggerFactory(new[] {
                  new DebugLoggerProvider()
              });

        protected string? GetCurrentSqlConnectionString()
            => TestSqlContext?.Database.GetDbConnection().ConnectionString;

    }

    //
    //  Summary:
    //    Integration base test that drops "Authorize" from all routes and adds a fake JWT user on every HttpContext.
    public class IntegrationTestBaseWithFakeJWT<TStartup> : IntegrationTestBase<TStartup> where TStartup : class
    {
        protected const string TEST_SELLER_ID = "d0164e85-8bfc-4138-37e3-08d68bda781d";

        private readonly Action<MvcOptions>? _setupAction;
        protected readonly FakeUserFilter DefaultFakeUserFilter = new FakeUserFilter(
            new List<Claim>()
            {
                new Claim("sellers", TEST_SELLER_ID),
                new Claim("cnpjs", "54794984000159"),
                new Claim("cnpjs", "78883819000131"),
                new Claim(ClaimTypes.NameIdentifier, "Dunno"),
                new Claim(ClaimTypes.Name, "Dunno"),
                new Claim(ClaimTypes.Email, "test@example.com"),
                new Claim(ClaimTypes.Role, "MasterClient"),
                new Claim(ClaimTypes.Webpage, "AdminApplication")
            }
        );

        public IntegrationTestBaseWithFakeJWT(bool configureServer = true, Action<MvcOptions>? setupAction = null)
            : base(configureServer && setupAction == null)
        {
            _setupAction = setupAction;

            if (configureServer && setupAction != null)
                ConfigureServer().GetAwaiter().GetResult();
        }

        protected override void AfterConfigureServices(IServiceCollection services)
        {
            base.AfterConfigureServices(services);

            // mocked user authentication with fake JWT
            services.AddSingleton<IAuthorizationHandler, AllowAnonymous>();
            services.AddMvc(_setupAction ?? (options =>
            {
                options.Filters.Add(new AllowAnonymousFilter());
                options.Filters.Add(DefaultFakeUserFilter);
            }));
        }

        public class AllowAnonymous : IAuthorizationHandler
        {
            public Task HandleAsync(AuthorizationHandlerContext context)
            {
                foreach (IAuthorizationRequirement requirement in context.PendingRequirements.ToList())
                    context.Succeed(requirement); //Simply pass all requirements

                return Task.CompletedTask;
            }
        }
    }
}
