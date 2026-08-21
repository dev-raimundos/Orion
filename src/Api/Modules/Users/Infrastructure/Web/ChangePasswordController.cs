using Microsoft.AspNetCore.Mvc;
using Api.Modules.Users.Application.UseCases;

namespace Api.Modules.Users.Infrastructure.Web;

[ApiController]
[Route("api/users")]
public class ChangePasswordController(ChangePasswordUseCase changePassword) : ControllerBase
{
    private readonly ChangePasswordUseCase _changePassword = changePassword;

    [HttpPut("{id:guid}/password")]
    public async Task<ActionResult<ChangePasswordResult>> ChangePassword(Guid id, ChangePasswordBody body, CancellationToken ct)
    {
        var result = await _changePassword.ExecuteAsync(new ChangePasswordRequest(id, body.CurrentPassword, body.NewPassword), ct);
        return Ok(result);
    }
}

public sealed record ChangePasswordBody(string CurrentPassword, string NewPassword);
