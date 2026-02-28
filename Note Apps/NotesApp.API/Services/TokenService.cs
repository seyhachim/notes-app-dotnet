using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using NotesApp.API.Models;

namespace NotesApp.API.Services;

/// <summary>
/// Responsible for generating JWT tokens.
/// Kept separate from AuthService because token generation
/// is its own concern — AuthService shouldn't know how tokens are built.
/// </summary>
public class TokenService(IConfiguration configuration)
{
    public string GenerateToken(User user)
    {
        // Claims are pieces of data embedded inside the token.
        // The API reads these on every request to know who is calling.
        // We store UserId and Email so we never need an extra DB lookup to identify the caller.
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
            // Jti is a unique ID per token — useful for token revocation later
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // The signing key — must match exactly what's in appsettings.json
        // In production this would come from a secrets manager, not appsettings
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(configuration["Jwt:Secret"]!)
        );

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            // Token expires after 24 hours — no refresh token needed for this project
            expires: DateTime.UtcNow.AddHours(
                double.Parse(configuration["Jwt:ExpiryHours"]!)
            ),
            signingCredentials: credentials
        );

        // Serializes the token to the string format: xxxxx.yyyyy.zzzzz
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}