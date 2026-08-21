using Authentication.Domain;

namespace Authentication.Domain.Abstractions;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken ct);
    Task AddAsync(RefreshToken token, CancellationToken ct);
    Task UpdateAsync(RefreshToken token, CancellationToken ct);
}
