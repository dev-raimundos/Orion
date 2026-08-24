using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Users.Application.UseCases;

namespace Users.Infrastructure.Web;

[ApiController]
[Route("api/users")]
[Authorize]
[Tags("Usuários")]
public class ChangePasswordController(ChangePasswordUseCase changePassword) : ControllerBase
{
    private readonly ChangePasswordUseCase _changePassword = changePassword;

    /// <summary>
    /// Altera a senha de um usuário, exigindo a senha atual como confirmação.
    /// </summary>
    /// <remarks>
    /// Requer um access token válido cujo Id (claim "sub") seja igual ao <paramref name="id"/> da rota.
    /// </remarks>
    [HttpPut("{id:guid}/password")]
    public async Task<ActionResult<ChangePasswordResult>> ChangePassword(Guid id, ChangePasswordBody body, CancellationToken ct)
    {
        this.EnsureIsCurrentUser(id);

        var result = await _changePassword.ExecuteAsync(new ChangePasswordRequest(id, body.CurrentPassword, body.NewPassword), ct);
        return Ok(result);
    }
}

public sealed record ChangePasswordBody(string CurrentPassword, string NewPassword);
