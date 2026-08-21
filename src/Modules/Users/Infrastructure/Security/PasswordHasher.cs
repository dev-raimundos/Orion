using Microsoft.AspNetCore.Identity;
using Users.Application.Abstractions;
using Users.Domain;

namespace Users.Infrastructure.Security;

public class PasswordHasher : IPasswordHasher
{
    private readonly PasswordHasher<User> _identityHasher = new();

    public string Hash(string password) =>
        _identityHasher.HashPassword(null!, password);

    public bool Verify(string password, string passwordHash) =>
        _identityHasher.VerifyHashedPassword(null!, passwordHash, password) != PasswordVerificationResult.Failed;
}
