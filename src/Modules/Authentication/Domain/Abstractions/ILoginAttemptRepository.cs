using Authentication.Domain;

namespace Authentication.Domain.Abstractions;

public interface ILoginAttemptRepository
{
    Task<IReadOnlyList<LoginAttempt>> GetRecentAsync(string email, DateTimeOffset since, CancellationToken ct);
    Task AddAsync(LoginAttempt attempt, CancellationToken ct);
}
