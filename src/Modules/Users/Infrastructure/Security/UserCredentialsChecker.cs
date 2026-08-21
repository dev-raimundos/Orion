using Orion.SharedKernel.Contracts;
using Users.Application.Abstractions;
using Users.Domain.Abstractions;

namespace Users.Infrastructure.Security;

public class UserCredentialsChecker(IUserRepository repository, IPasswordHasher passwordHasher) : IUserCredentialsChecker
{
    private readonly IUserRepository _repository = repository;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;

    public async Task<AuthenticatedUser?> ValidateAsync(string email, string password, CancellationToken ct)
    {
        var user = await _repository.GetByEmailAsync(email, ct);

        if (user is null || !user.Active || !_passwordHasher.Verify(password, user.PasswordHash))
            return null;

        return new AuthenticatedUser(user.Id, user.Email);
    }
}
