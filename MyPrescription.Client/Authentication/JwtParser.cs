using System.Security.Claims;
using System.Text.Json;
using System.IdentityModel.Tokens.Jwt;

namespace MyPrescription.Client.Authentication;

public class JwtParser
{

    public static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var claims = new List<Claim>();
        var payload = jwt.Split('.')[1];

        var jsonBytes = ParseBase64WithoutPadding(payload);

        var kvp = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
        ExtractRolesFromJWT(claims, kvp!);

        claims.AddRange(kvp!.Select(x => new Claim(x.Key, x.Value.ToString()!)));

        return claims;
    }


    private static void ExtractRolesFromJWT(List<Claim> claims, Dictionary<string, object> keyValuePairs)
    {
        keyValuePairs.TryGetValue(ClaimTypes.Role, out object? roles);
        if (roles is not null)
        {
            var parsedRoles = roles.ToString()!.Trim().TrimStart('[').TrimEnd(']').Split(',');
            if (parsedRoles.Length > 1)
            {
                foreach (var parsedRole in parsedRoles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, parsedRole.Trim('"')));
                }
            }
            else
            {
                claims.Add(new Claim(ClaimTypes.Role, parsedRoles[0]));
            }
            keyValuePairs.Remove(ClaimTypes.Role);
        }
    }

    private static byte[] ParseBase64WithoutPadding(string base64)
    {
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }
        return Convert.FromBase64String(base64);
    }

    public static bool IsValid(string token)
    {
        JwtSecurityToken jwtSecurityToken;
        try
        {
            jwtSecurityToken = new JwtSecurityToken(token);
        }
        catch (Exception)
        {
            return false;
        }
        return jwtSecurityToken.ValidTo > DateTime.UtcNow;
    }
}
