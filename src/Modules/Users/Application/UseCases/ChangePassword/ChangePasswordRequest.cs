namespace Users.Application.UseCases.ChangePassword;

public sealed record ChangePasswordRequest(Guid UserId, string CurrentPassword, string NewPassword);
