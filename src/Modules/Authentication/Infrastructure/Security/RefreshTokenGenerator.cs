using System.Security.Cryptography;
using System.Text;
using Authentication.Application.Abstractions;

namespace Authentication.Infrastructure.Security;

public class RefreshTokenGenerator : IRefreshTokenGenerator
{
    private const int TokenSizeInBytes = 64;

    public (string Token, string TokenHash) Generate()
    {
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(TokenSizeInBytes));
        return (token, Hash(token));
    }

    public string Hash(string token) =>
        Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
}
