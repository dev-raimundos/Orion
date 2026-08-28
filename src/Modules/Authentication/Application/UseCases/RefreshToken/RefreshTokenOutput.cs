namespace Authentication.Application.UseCases.RefreshToken;

public sealed record RefreshTokenOutput(string AccessToken, DateTimeOffset ExpiresAt, string RefreshToken);
