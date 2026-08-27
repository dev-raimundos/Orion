using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Users.Application.UseCases.GetUserById;

namespace Users.Infrastructure.Web;

[ApiController]
[Route("api/users")]
[Authorize]
[Tags("Usuários")]
public class GetUserByIdController(GetUserByIdUseCase getUserById) : ControllerBase
{
    private readonly GetUserByIdUseCase _getUserById = getUserById;

    /// <summary>
    /// Retorna os dados de um usuário pelo Id.
    /// </summary>
    /// <remarks>
    /// Requer um access token válido cujo Id (claim "sub") seja igual ao <paramref name="id"/> da rota.
    /// </remarks>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserResponse>> GetById(Guid id, CancellationToken ct)
    {
        this.EnsureIsCurrentUser(id);

        var result = await _getUserById.ExecuteAsync(new GetUserByIdRequest(id), ct);
        return Ok(result);
    }
}
