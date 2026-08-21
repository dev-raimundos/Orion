namespace Authentication.Application.Abstractions;

public interface ITokenGenerator
{
    (string Token, DateTimeOffset ExpiresAt) Generate(Guid userId, string email);
}
