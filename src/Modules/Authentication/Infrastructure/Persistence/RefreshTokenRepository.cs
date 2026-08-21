using Microsoft.EntityFrameworkCore;
using Authentication.Domain;
using Authentication.Domain.Abstractions;

namespace Authentication.Infrastructure.Persistence;

public class RefreshTokenRepository(AuthenticationDbContext context) : IRefreshTokenRepository
{
    private readonly AuthenticationDbContext _context = context;

    public Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken ct) =>
        _context.RefreshTokens.FirstOrDefaultAsync(t => t.TokenHash == tokenHash, ct);

    public async Task AddAsync(RefreshToken token, CancellationToken ct)
    {
        await _context.RefreshTokens.AddAsync(token, ct);
        await _context.SaveChangesAsync(ct);
    }

    // Assume que 'token' foi carregado por este mesmo context (GetByTokenHashAsync), então já está tracked.
    public async Task UpdateAsync(RefreshToken token, CancellationToken ct)
    {
        await _context.SaveChangesAsync(ct);
    }
}
