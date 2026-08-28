namespace Authentication.Domain;

public class LoginAttempt
{
    public Guid Id { get; private set; }
    public string Email { get; private set; } = null!;
    public bool Succeeded { get; private set; }
    public DateTimeOffset AttemptedAt { get; private set; }

    public LoginAttempt()
    {
    }

    public LoginAttempt(string email, bool succeeded)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email não pode ser vazio.", nameof(email));

        Id = Guid.NewGuid();
        Email = email;
        Succeeded = succeeded;
        AttemptedAt = DateTimeOffset.UtcNow;
    }
}
