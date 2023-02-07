using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Acropolis.Api.Extensions.Authentication;

public class ConfigureJwtOptions : IConfigureNamedOptions<JwtBearerOptions>
{
    private readonly TokenConfig tokenConfig;

    public ConfigureJwtOptions(IConfiguration configuration)
    {
        tokenConfig = new TokenConfig();
        configuration.Bind("TokenCredentials", tokenConfig);
        if (string.IsNullOrEmpty(tokenConfig.HmacSecretKey))
        {
            throw new Exception("HmacSecretKey is not definied. Perhaps TokenCredentials is not presented at appsettings.json.");
        }
    }

    public void Configure(string name, JwtBearerOptions options)
    {
        TokenValidationParameters tokenValidationParameters = options.TokenValidationParameters;
        tokenValidationParameters.ValidateIssuerSigningKey = true;
        tokenValidationParameters.ValidateLifetime = true;
        tokenValidationParameters.ClockSkew = TimeSpan.Zero;
        tokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenConfig.HmacSecretKey));
        tokenValidationParameters.ValidAudience = tokenConfig.Audience;
        tokenValidationParameters.ValidIssuer = tokenConfig.Issuer;
    }
    public void Configure(JwtBearerOptions options)
    {
        Configure(Options.DefaultName, options);
    }
}