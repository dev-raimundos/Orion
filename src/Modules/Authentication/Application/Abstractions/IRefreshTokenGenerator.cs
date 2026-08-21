namespace Authentication.Application.Abstractions;

public interface IRefreshTokenGenerator
{
    (string Token, string TokenHash) Generate();

    string Hash(string token);
}
