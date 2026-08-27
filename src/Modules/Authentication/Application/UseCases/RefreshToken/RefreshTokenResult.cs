namespace Authentication.Application.UseCases.RefreshToken;

public sealed record RefreshTokenResult(string AccessToken, DateTimeOffset ExpiresAt, string RefreshToken);
