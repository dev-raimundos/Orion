namespace Users.Application.UseCases.VerifyEmail;

public sealed record VerifyEmailResult(Guid Id, bool EmailVerified);
