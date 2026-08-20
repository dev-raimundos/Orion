using Orion.SharedKernel;

namespace Users.Domain;

public class User : Entity<Guid>
{
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public bool Active { get; private set; }
    public bool EmailVerified { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public DateTimeOffset? LastLoginAt { get; private set; }

    private User()
    {
    }

    public static User Create(string name, string email, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Nome não pode ser vazio.", nameof(name));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email não pode ser vazio.", nameof(email));

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash não pode ser vazio.", nameof(passwordHash));

        var now = DateTimeOffset.UtcNow;

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = name,
            Email = email,
            PasswordHash = passwordHash,
            Active = true,
            EmailVerified = false,
            CreatedAt = now,
            UpdatedAt = now,
            LastLoginAt = null
        };

        return user;
    }

    public void Rename(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("Nome não pode ser vazio.", nameof(newName));

        Name = newName;
        Touch();
    }

    public void ChangePassword(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
            throw new ArgumentException("Password hash não pode ser vazio.", nameof(newPasswordHash));

        PasswordHash = newPasswordHash;
        Touch();
    }

    public void VerifyEmail()
    {
        if (EmailVerified)
            return;

        EmailVerified = true;
        Touch();
    }

    public void Deactivate()
    {
        if (!Active)
            return;

        Active = false;
        Touch();
    }

    public void Activate()
    {
        if (Active)
            return;

        Active = true;
        Touch();
    }

    public void RegisterLogin()
    {
        LastLoginAt = DateTimeOffset.UtcNow;
    }

    private void Touch() => UpdatedAt = DateTimeOffset.UtcNow;
}