using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Acropolis.Tests.Helpers.Api
{
    public static class JwtMock
    {
        public static string Issuer { get; }
        public static string Audience { get; }
        public static SecurityKey SecurityKey { get; }
        public static SigningCredentials SigningCredentials { get; }

        private static readonly JwtSecurityTokenHandler _tokenHandler = new JwtSecurityTokenHandler();

        static JwtMock()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .AddEnvironmentVariables()
                .Build();
            
            configuration["TokenCredentials:HmacSecretKey"] = "fake-hmac-secret";

            Issuer = configuration["TokenCredentials:Issuer"];
            Audience = configuration["TokenCredentials:Audience"];
            SecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["TokenCredentials:HmacSecretKey"])) { KeyId = Guid.NewGuid().ToString() };
            SigningCredentials = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256Signature);
        }

        public static string GenerateJwtToken(Dictionary<string, string> claims)
            => GenerateJwtToken(claims.Select(entry => new Claim(entry.Key, entry.Value)).ToList());

        public static string GenerateJwtToken(IEnumerable<Claim> claims)
        {
            return _tokenHandler.WriteToken(new JwtSecurityToken(Issuer, Audience, claims, null, DateTime.UtcNow.AddHours(60), SigningCredentials));
        }
    }
}
