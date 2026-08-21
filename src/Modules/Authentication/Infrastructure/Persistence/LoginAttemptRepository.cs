using Microsoft.EntityFrameworkCore;
using Authentication.Domain;
using Authentication.Domain.Abstractions;

namespace Authentication.Infrastructure.Persistence;

public class LoginAttemptRepository(AuthenticationDbContext context) : ILoginAttemptRepository
{
    private readonly AuthenticationDbContext _context = context;

    public async Task<IReadOnlyList<LoginAttempt>> GetRecentAsync(string email, DateTimeOffset since, CancellationToken ct) =>
        await _context.LoginAttempts
            .Where(a => a.Email == email && a.AttemptedAt >= since)
            .ToListAsync(ct);

    public async Task AddAsync(LoginAttempt attempt, CancellationToken ct)
    {
        await _context.LoginAttempts.AddAsync(attempt, ct);
        await _context.SaveChangesAsync(ct);
    }
}
