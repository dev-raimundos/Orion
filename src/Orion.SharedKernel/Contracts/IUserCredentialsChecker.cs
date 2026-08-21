namespace Orion.SharedKernel.Contracts;

public sealed record AuthenticatedUser(Guid Id, string Email);

public interface IUserCredentialsChecker
{
    Task<AuthenticatedUser?> ValidateAsync(string email, string password, CancellationToken ct);
}
