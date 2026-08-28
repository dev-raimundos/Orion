using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Users.Application.UseCases.RenameUser;

namespace Users.Infrastructure.Web;

[ApiController]
[Route("api/users")]
[Authorize]
[Tags("Usuários")]
public class RenameUserController(RenameUserUseCase renameUser) : ControllerBase
{
    private readonly RenameUserUseCase _renameUser = renameUser;

    /// <summary>
    /// Altera o nome de um usuário existente.
    /// </summary>
    /// <remarks>
    /// Requer um access token válido cujo Id (claim "sub") seja igual ao <paramref name="id"/> da rota.
    /// </remarks>
    [HttpPut("{id:guid}/name")]
    public async Task<ActionResult<RenameUserOutput>> Rename(Guid id, [FromBody] string newName, CancellationToken ct)
    {
        this.EnsureIsCurrentUser(id);

        var result = await _renameUser.ExecuteAsync(new RenameUserInput(id, newName), ct);
        return Ok(result);
    }
}
