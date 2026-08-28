namespace Authentication.Domain;

public class RefreshToken
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string Email { get; private set; } = null!;
    public string TokenHash { get; private set; } = null!;
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset ExpiresAt { get; private set; }
    public DateTimeOffset? RevokedAt { get; private set; }

    public bool IsActive => RevokedAt is null && ExpiresAt > DateTimeOffset.UtcNow;

    public RefreshToken()
    {
    }

    public RefreshToken(Guid userId, string email, string tokenHash, TimeSpan lifetime)
    {
        var now = DateTimeOffset.UtcNow;

        Id = Guid.NewGuid();
        UserId = userId;
        Email = email;
        TokenHash = tokenHash;
        CreatedAt = now;
        ExpiresAt = now + lifetime;
        RevokedAt = null;
    }

    public void Revoke() => RevokedAt ??= DateTimeOffset.UtcNow;
}
