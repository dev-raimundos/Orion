namespace Users.Application.UseCases.DeactivateUser;

public sealed record DeactivateUserOutput(Guid Id, bool Active);
