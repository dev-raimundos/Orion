using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Authentication.Application.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Authentication.Infrastructure.Security;

public class JwtTokenGenerator(IConfiguration configuration) : ITokenGenerator
{
    private readonly IConfiguration _configuration = configuration;

    public (string Token, DateTimeOffset ExpiresAt) Generate(Guid userId, string email)
    {
        var jwtSection = _configuration.GetSection("Jwt");

        var signingKey = jwtSection["SigningKey"]
            ?? throw new InvalidOperationException("Configuração 'Jwt:SigningKey' não configurada.");

        var issuer = jwtSection["Issuer"];
        var audience = jwtSection["Audience"];
        var expirationMinutes = jwtSection.GetValue<int?>("ExpirationMinutes") ?? 60;

        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(expirationMinutes);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiresAt.UtcDateTime,
            signingCredentials: credentials);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }
}
