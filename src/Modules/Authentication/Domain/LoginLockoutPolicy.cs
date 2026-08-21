namespace Authentication.Domain;

public static class LoginLockoutPolicy
{
    public const int MaxFailedAttempts = 5;
    public static readonly TimeSpan Window = TimeSpan.FromMinutes(15);

    public static bool IsLockedOut(IReadOnlyCollection<LoginAttempt> recentAttempts, DateTimeOffset now, out DateTimeOffset? lockedUntil)
    {
        var failures = recentAttempts
            .Where(a => !a.Succeeded)
            .OrderByDescending(a => a.AttemptedAt)
            .ToList();

        if (failures.Count < MaxFailedAttempts)
        {
            lockedUntil = null;
            return false;
        }

        lockedUntil = failures[0].AttemptedAt + Window;
        return now < lockedUntil;
    }
}
