using Orion.SharedKernel;

using Authentication.Application.Abstractions;
using Authentication.Domain;
using Authentication.Domain.Abstractions;

namespace Authentication.Application.UseCases.RefreshToken;

public class RefreshTokenUseCase(
    IRefreshTokenRepository refreshTokens,
    IRefreshTokenGenerator refreshTokenGenerator,
    ITokenGenerator tokenGenerator)
{
    private readonly IRefreshTokenRepository _refreshTokens = refreshTokens;
    private readonly IRefreshTokenGenerator _refreshTokenGenerator = refreshTokenGenerator;
    private readonly ITokenGenerator _tokenGenerator = tokenGenerator;

    public async Task<RefreshTokenResult> ExecuteAsync(RefreshTokenRequest request, CancellationToken ct)
    {
        var tokenHash = _refreshTokenGenerator.Hash(request.RefreshToken);

        var existing = await _refreshTokens.GetByTokenHashAsync(tokenHash, ct);

        if (existing is null || !existing.IsActive)
            throw new AppUnauthorizedException("Refresh token inválido ou expirado.");

        existing.Revoke();

        await _refreshTokens.UpdateAsync(existing, ct);

        var (newRawToken, newTokenHash) = _refreshTokenGenerator.Generate();

        var newRefreshToken = Authentication.Domain.RefreshToken.Create(
            existing.UserId,
            existing.Email,
            newTokenHash,
            RefreshTokenPolicy.Lifetime
        );

        await _refreshTokens.AddAsync(newRefreshToken, ct);

        var (accessToken, expiresAt) = _tokenGenerator
            .Generate(existing.UserId, existing.Email);

        return new RefreshTokenResult(
            accessToken,
            expiresAt,
            newRawToken
        );
    }
}
