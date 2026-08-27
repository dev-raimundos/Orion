using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Users.Application.UseCases.ActivateUser;

namespace Users.Infrastructure.Web;

[ApiController]
[Route("api/users")]
[Authorize]
[Tags("Usuários")]
public class ActivateUserController(ActivateUserUseCase activateUser) : ControllerBase
{
    private readonly ActivateUserUseCase _activateUser = activateUser;

    /// <summary>
    /// Reativa uma conta de usuário previamente desativada.
    /// </summary>
    /// <remarks>
    /// Requer um access token válido cujo Id (claim "sub") seja igual ao <paramref name="id"/> da rota.
    /// </remarks>
    [HttpPost("{id:guid}/activate")]
    public async Task<ActionResult<ActivateUserResult>> Activate(Guid id, CancellationToken ct)
    {
        this.EnsureIsCurrentUser(id);

        var result = await _activateUser.ExecuteAsync(new ActivateUserRequest(id), ct);
        return Ok(result);
    }
}
