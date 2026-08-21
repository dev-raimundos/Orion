using Authentication.Application.Abstractions;
using Authentication.Domain.Abstractions;

namespace Authentication.Application.UseCases;

public sealed record LogoutRequest(string RefreshToken);

public class LogoutUseCase(IRefreshTokenRepository refreshTokens, IRefreshTokenGenerator refreshTokenGenerator)
{
    private readonly IRefreshTokenRepository _refreshTokens = refreshTokens;
    private readonly IRefreshTokenGenerator _refreshTokenGenerator = refreshTokenGenerator;

    public async Task ExecuteAsync(LogoutRequest request, CancellationToken ct)
    {
        var tokenHash = _refreshTokenGenerator.Hash(request.RefreshToken);
        var existing = await _refreshTokens.GetByTokenHashAsync(tokenHash, ct);

        if (existing is null)
            return;

        existing.Revoke();
        await _refreshTokens.UpdateAsync(existing, ct);
    }
}
