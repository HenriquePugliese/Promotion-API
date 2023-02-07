using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;

namespace Acropolis.Api.Extensions.Authentication;

public static class JsmAuthenticationExtension
{
    public static AuthenticationBuilder AddJSMAuthentication(this IServiceCollection services)
    {
        return services.AddAuthentication(delegate (AuthenticationOptions sharedOptions)
        {
            sharedOptions.DefaultScheme = "Bearer";
            sharedOptions.DefaultChallengeScheme = "Bearer";
            sharedOptions.DefaultAuthenticateScheme = "Bearer";
        }).AddJSMAuthentication();
    }
    public static AuthenticationBuilder AddJSMAuthentication(this AuthenticationBuilder builder)
    {
        return builder.AddJSMAuthentication(delegate
        {
        });
    }
    public static AuthenticationBuilder AddJSMAuthentication(this AuthenticationBuilder builder, Action<TokenConfig> configureOptions)
    {
        builder.Services.Configure(configureOptions);
        builder.Services.AddSingleton<IConfigureOptions<JwtBearerOptions>, ConfigureJwtOptions>();
        builder.AddJwtBearer();
        return builder;
    }
}
