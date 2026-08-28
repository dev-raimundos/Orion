namespace Users.Application.UseCases.VerifyEmail;

public sealed record VerifyEmailOutput(Guid Id, bool EmailVerified);
