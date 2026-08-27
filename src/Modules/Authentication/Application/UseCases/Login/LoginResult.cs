namespace Authentication.Application.UseCases.Login;

public sealed record LoginResult(string AccessToken, DateTimeOffset ExpiresAt, string RefreshToken);
