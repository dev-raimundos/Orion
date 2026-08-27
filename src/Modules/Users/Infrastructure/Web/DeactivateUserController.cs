using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Users.Application.UseCases.DeactivateUser;

namespace Users.Infrastructure.Web;

[ApiController]
[Route("api/users")]
[Authorize]
[Tags("Usuários")]
public class DeactivateUserController(DeactivateUserUseCase deactivateUser) : ControllerBase
{
    private readonly DeactivateUserUseCase _deactivateUser = deactivateUser;

    /// <summary>
    /// Desativa a conta de um usuário.
    /// </summary>
    /// <remarks>
    /// Requer um access token válido cujo Id (claim "sub") seja igual ao <paramref name="id"/> da rota.
    /// </remarks>
    [HttpPost("{id:guid}/deactivate")]
    public async Task<ActionResult<DeactivateUserResult>> Deactivate(Guid id, CancellationToken ct)
    {
        this.EnsureIsCurrentUser(id);

        var result = await _deactivateUser.ExecuteAsync(new DeactivateUserRequest(id), ct);
        return Ok(result);
    }
}
