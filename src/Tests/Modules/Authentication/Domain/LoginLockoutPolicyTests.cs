using Authentication.Domain;

namespace Authentication.Tests.Domain;

public class LoginLockoutPolicyTests
{
    [Fact]
    public void IsLockedOut_WhenFewerThanMaxFailedAttempts_ReturnsFalse()
    {
        var attempts = Enumerable.Range(0, LoginLockoutPolicy.MaxFailedAttempts - 1)
            .Select(_ => new LoginAttempt("fulano@teste.com", succeeded: false))
            .ToList();

        var isLockedOut = LoginLockoutPolicy.IsLockedOut(attempts, DateTimeOffset.UtcNow, out var lockedUntil);

        Assert.False(isLockedOut);
        Assert.Null(lockedUntil);
    }

    [Fact]
    public void IsLockedOut_WhenMaxFailedAttemptsHappenedJustNow_ReturnsTrue()
    {
        var attempts = Enumerable.Range(0, LoginLockoutPolicy.MaxFailedAttempts)
            .Select(_ => new LoginAttempt("fulano@teste.com", succeeded: false))
            .ToList();

        var isLockedOut = LoginLockoutPolicy.IsLockedOut(attempts, DateTimeOffset.UtcNow, out var lockedUntil);

        Assert.True(isLockedOut);
        Assert.NotNull(lockedUntil);
    }

    [Fact]
    public void IsLockedOut_SuccessfulAttemptsDoNotCountTowardLockout()
    {
        var attempts = Enumerable.Range(0, LoginLockoutPolicy.MaxFailedAttempts)
            .Select(_ => new LoginAttempt("fulano@teste.com", succeeded: true))
            .ToList();

        var isLockedOut = LoginLockoutPolicy.IsLockedOut(attempts, DateTimeOffset.UtcNow, out var lockedUntil);

        Assert.False(isLockedOut);
        Assert.Null(lockedUntil);
    }
}
