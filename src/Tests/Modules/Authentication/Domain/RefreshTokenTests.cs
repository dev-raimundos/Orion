using Authentication.Domain;

namespace Authentication.Tests.Domain;

public class RefreshTokenTests
{
    [Fact]
    public void Constructor_ReturnsAnActiveToken()
    {
        var token = new RefreshToken(Guid.NewGuid(), "fulano@teste.com", "hash", TimeSpan.FromDays(7));

        Assert.True(token.IsActive);
    }

    [Fact]
    public void IsActive_WhenExpired_ReturnsFalse()
    {
        var token = new RefreshToken(Guid.NewGuid(), "fulano@teste.com", "hash", TimeSpan.FromSeconds(-1));

        Assert.False(token.IsActive);
    }

    [Fact]
    public void IsActive_WhenRevoked_ReturnsFalse()
    {
        var token = new RefreshToken(Guid.NewGuid(), "fulano@teste.com", "hash", TimeSpan.FromDays(7));

        token.Revoke();

        Assert.False(token.IsActive);
    }

    [Fact]
    public void Revoke_WhenCalledTwice_KeepsTheFirstRevocationTimestamp()
    {
        var token = new RefreshToken(Guid.NewGuid(), "fulano@teste.com", "hash", TimeSpan.FromDays(7));

        token.Revoke();
        var firstRevokedAt = token.RevokedAt;
        token.Revoke();

        Assert.Equal(firstRevokedAt, token.RevokedAt);
    }
}
