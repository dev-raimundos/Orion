using Authentication.Application.Abstractions;
using Authentication.Domain;
using Authentication.Domain.Abstractions;

using Orion.SharedKernel;
using Orion.SharedKernel.Contracts;

namespace Authentication.Application.UseCases.Login;

public class LoginUseCase(
    IUserCredentialsChecker credentialsChecker,
    ITokenGenerator tokenGenerator,
    IRefreshTokenGenerator refreshTokenGenerator,
    ILoginAttemptRepository loginAttempts,
    IRefreshTokenRepository refreshTokens)
{
    private readonly IUserCredentialsChecker _credentialsChecker = credentialsChecker;
    private readonly ITokenGenerator _tokenGenerator = tokenGenerator;
    private readonly IRefreshTokenGenerator _refreshTokenGenerator = refreshTokenGenerator;
    private readonly ILoginAttemptRepository _loginAttempts = loginAttempts;
    private readonly IRefreshTokenRepository _refreshTokens = refreshTokens;

    public async Task<LoginOutput> ExecuteAsync(LoginInput request, CancellationToken ct)
    {
        var now = DateTimeOffset.UtcNow;

        var recentAttempts = await _loginAttempts.GetRecentAsync(
            request.Email,
            now - LoginLockoutPolicy.Window, ct
        );

        if (LoginLockoutPolicy.IsLockedOut(recentAttempts, now, out var lockedUntil))
        {
            throw new AppLockedException(
                $"Muitas tentativas de login falharam. Tente novamente após {lockedUntil:HH:mm:ss} UTC."
            );
        }

        var authenticatedUser = await _credentialsChecker.ValidateAsync(
            request.Email,
            request.Password, ct
        );

        await _loginAttempts.AddAsync(new LoginAttempt(
            request.Email, succeeded: authenticatedUser is not null),
            ct
        );

        if (authenticatedUser is null)
            throw new AppUnauthorizedException("Email ou senha inválidos.");

        var (accessToken, expiresAt) = _tokenGenerator
            .Generate(authenticatedUser.Id, authenticatedUser.Email);

        var (refreshToken, refreshTokenHash) = _refreshTokenGenerator.Generate();

        var refreshTokenEntity = new Authentication.Domain.RefreshToken(
            authenticatedUser.Id,
            authenticatedUser.Email,
            refreshTokenHash,
            RefreshTokenPolicy.Lifetime
        );

        await _refreshTokens.AddAsync(refreshTokenEntity, ct);

        return new LoginOutput(
            accessToken,
            expiresAt,
            refreshToken
        );
    }
}
