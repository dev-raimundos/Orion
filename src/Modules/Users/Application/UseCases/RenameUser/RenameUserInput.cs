namespace Users.Application.UseCases.RenameUser;

public sealed record RenameUserInput(Guid UserId, string NewName);
