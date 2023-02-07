namespace Acropolis.Api.Extensions.Authentication;

public class TokenConfig
{
    public string Audience { get; set; } = null!;
    public string Issuer { get; set; } = null!;
    public string HmacSecretKey { get; set; } = null!;
}
