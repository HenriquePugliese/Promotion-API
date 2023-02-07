using System.Security.Claims;

namespace Acropolis.Api.Extensions.Authentication;

public static class ClaimsPrincipalExtension
{
    public static bool IsUserJSMAdmin(this ClaimsPrincipal claimsPrincipal)
    {
        claimsPrincipal.CheckClaimsPrincipal();

        return claimsPrincipal.HasClaim(t => t.Type == ClaimTypes.Role && t.Value == "JSMAdmin");
    }

    public static IEnumerable<string> GetRolesFromToken(this ClaimsPrincipal claimsPrincipal)
    {
        claimsPrincipal.CheckClaimsPrincipal();

        var claims = claimsPrincipal.FindAll(t => t.Type == ClaimTypes.Role);

        if (!claims.Any()) return new List<string>();

        return claims.Select(t => t.Value);
    }

    public static IEnumerable<string> GetCpfsFromToken(this ClaimsPrincipal claimsPrincipal)
    {
        claimsPrincipal.CheckClaimsPrincipal();

        var claims = claimsPrincipal.FindAll(t => t.Type == "cpfs");

        if (!claims.Any()) return new List<string>();

        return claims.Select(t => t.Value);
    }

    public static IEnumerable<string> GetCnpjsFromToken(this ClaimsPrincipal claimsPrincipal)
    {
        claimsPrincipal.CheckClaimsPrincipal();

        var claims = claimsPrincipal.FindAll(t => t.Type == "cnpjs");

        if (!claims.Any()) return new List<string>();

        return claims.Select(t => t.Value);
    }

    public static string? GetEmailFromToken(this ClaimsPrincipal claimsPrincipal)
    {
        claimsPrincipal.CheckClaimsPrincipal();

        var claims = claimsPrincipal.FindAll(t => t.Type == ClaimTypes.Email);

        return claims.FirstOrDefault()?.Value;
    }

    public static bool CheckLoginAccess(this ClaimsPrincipal claimsPrincipal)
    {
        claimsPrincipal.CheckClaimsPrincipal();

        var claims = claimsPrincipal.FindAll(t => t.Type == "fid");

        if (!claims.Any()) return true;

        return false;
    }
    public static string? GetFirstSellerIdFromToken(this ClaimsPrincipal claimsPrincipal)
    {
        claimsPrincipal.CheckClaimsPrincipal();

        var claims = claimsPrincipal.FindAll(t => t.Type == "sellers");

        if (!claims.Any()) return string.Empty;

        return claims.FirstOrDefault()?.Value;
    }

    public static string? GetFirstCnpjFromToken(this ClaimsPrincipal claimsPrincipal)
    {
        claimsPrincipal.CheckClaimsPrincipal();

        var claims = claimsPrincipal.FindAll(t => t.Type == "cnpjs");

        if (!claims.Any()) return string.Empty;

        return claims.FirstOrDefault()?.Value;
    }

    public static string? GetEmployeeIdFromToken(this ClaimsPrincipal claimsPrincipal)
    {
        claimsPrincipal.CheckClaimsPrincipal();

        var claims = claimsPrincipal.FindAll(t => t.Type == "uid");

        return claims.FirstOrDefault()?.Value;
    }

    public static string? GetNameFromToken(this ClaimsPrincipal claimsPrincipal)
    {
        claimsPrincipal.CheckClaimsPrincipal();

        var claims = claimsPrincipal.FindAll(t => t.Type == ClaimTypes.Name);

        return claims.FirstOrDefault()?.Value;
    }

    public static string? GetAppTypeFromToken(this ClaimsPrincipal claimsPrincipal)
    {
        claimsPrincipal.CheckClaimsPrincipal();

        var claims = claimsPrincipal.FindAll(t => t.Type == ClaimTypes.Webpage);

        if (!claims.Any()) return "NoAccess";

        return claims.FirstOrDefault()?.Value;
    }

    public static bool HasUserAccessTo(this ClaimsPrincipal claimsPrincipal, EnumRegistryType registryType)
    {
        claimsPrincipal.CheckClaimsPrincipal();

        var claims = claimsPrincipal.FindAll(t => t.Type == ClaimTypes.Webpage);

        if (!claims.Any()) return registryType == EnumRegistryType.NoAccess;

        return claims.Any(t => (System.Enum.Parse<EnumRegistryType>(t.Value) & registryType) != 0);
    }

    public static bool HasUserAccessToAdmin(this ClaimsPrincipal claimsPrincipal) => claimsPrincipal.HasUserAccessTo(EnumRegistryType.AdminApplication);
    public static bool HasUserAccessToClient(this ClaimsPrincipal claimsPrincipal) => claimsPrincipal.HasUserAccessTo(EnumRegistryType.ClientApplication);

    public static void CheckClaimsPrincipal(this ClaimsPrincipal claimsPrincipal)
    {
        if (claimsPrincipal == null) throw new Exception("There is no User. Probably [Authorize] is missing.");
    }
}
