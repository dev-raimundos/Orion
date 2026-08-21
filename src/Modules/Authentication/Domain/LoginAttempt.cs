using Orion.SharedKernel;

namespace Authentication.Domain;

public class LoginAttempt : Entity<Guid>
{
    public string Email { get; private set; }
    public bool Succeeded { get; private set; }
    public DateTimeOffset AttemptedAt { get; private set; }

    private LoginAttempt()
    {
    }

    public static LoginAttempt Record(string email, bool succeeded)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email não pode ser vazio.", nameof(email));

        return new LoginAttempt
        {
            Id = Guid.NewGuid(),
            Email = email,
            Succeeded = succeeded,
            AttemptedAt = DateTimeOffset.UtcNow
        };
    }
}
