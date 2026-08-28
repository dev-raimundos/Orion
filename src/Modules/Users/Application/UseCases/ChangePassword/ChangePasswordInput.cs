namespace Users.Application.UseCases.ChangePassword;

public sealed record ChangePasswordInput(Guid UserId, string CurrentPassword, string NewPassword);
