namespace Users.Application.UseCases.DeactivateUser;

public sealed record DeactivateUserResult(Guid Id, bool Active);
