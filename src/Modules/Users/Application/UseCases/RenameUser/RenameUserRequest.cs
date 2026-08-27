namespace Users.Application.UseCases.RenameUser;

public sealed record RenameUserRequest(Guid UserId, string NewName);
