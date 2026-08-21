using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Users.Application.UseCases;

namespace Users.Infrastructure.Web;

[ApiController]
[Route("api/users")]
[Authorize]
public class ChangePasswordController(ChangePasswordUseCase changePassword) : ControllerBase
{
    private readonly ChangePasswordUseCase _changePassword = changePassword;

    [HttpPut("{id:guid}/password")]
    public async Task<ActionResult<ChangePasswordResult>> ChangePassword(Guid id, ChangePasswordBody body, CancellationToken ct)
    {
        this.EnsureIsCurrentUser(id);

        var result = await _changePassword.ExecuteAsync(new ChangePasswordRequest(id, body.CurrentPassword, body.NewPassword), ct);
        return Ok(result);
    }
}

public sealed record ChangePasswordBody(string CurrentPassword, string NewPassword);
