namespace Authentication.Domain;

public static class RefreshTokenPolicy
{
    public static readonly TimeSpan Lifetime = TimeSpan.FromDays(7);
}
