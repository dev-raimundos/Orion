using Orion.Application;
using Orion.Application.Contracts;
using Authentication.Application.Abstractions;

namespace Authentication.Application.UseCases;

public sealed record LoginRequest(string Email, string Password);
public sealed record LoginResult(string AccessToken, DateTimeOffset ExpiresAt);

public class LoginUseCase(IUserCredentialsChecker credentialsChecker, ITokenGenerator tokenGenerator)
{
    private readonly IUserCredentialsChecker _credentialsChecker = credentialsChecker;
    private readonly ITokenGenerator _tokenGenerator = tokenGenerator;

    public async Task<LoginResult> ExecuteAsync(LoginRequest request, CancellationToken ct)
    {
        var authenticatedUser = await _credentialsChecker.ValidateAsync(request.Email, request.Password, ct)
            ?? throw new AppUnauthorizedException("Email ou senha inválidos.");

        var (token, expiresAt) = _tokenGenerator.Generate(authenticatedUser.Id, authenticatedUser.Email);

        return new LoginResult(token, expiresAt);
    }
}
