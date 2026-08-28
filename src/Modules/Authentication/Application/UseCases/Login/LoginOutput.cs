namespace Authentication.Application.UseCases.Login;

public sealed record LoginOutput(string AccessToken, DateTimeOffset ExpiresAt, string RefreshToken);
