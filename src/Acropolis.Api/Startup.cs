using Acropolis.Api.Extensions;
using Acropolis.Api.Extensions.Authentication;
using CustomExceptionMiddleware;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace Acropolis.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }

        private readonly IWebHostEnvironment _env;
        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            
            services.AddJSMAuthentication();
            services.AddEndpointsApiExplorer();
            services.AddSwagger();
            services.AddVersioning();
            services.AddValidations();
            services.AddApplication(Configuration);
            services.AddPersistence(Configuration, _env);
            
            services.AddHealthChecksSetup(Configuration);
            
            services.RegisterCapServicesResolver(Configuration);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(x => x
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseSwagger(c =>
            {
                c.RouteTemplate = "/cosmopolitan/documentation/{documentName}/swagger.json";
            });
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/cosmopolitan/documentation/v1/swagger.json", "API V1"));

            app.UseCustomExceptionMiddleware();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();          

            app.UseAuthorization();            

            app.UseHealthChecks("/health/liveness", new HealthCheckOptions
            {
                Predicate = _ => false,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            app.UseHealthChecks("/health", new HealthCheckOptions()
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
