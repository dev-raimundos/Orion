using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Users.Application.UseCases;

namespace Users.Infrastructure.Web;

[ApiController]
[Route("api/users")]
[Authorize]
[EnableRateLimiting("auth")]
[Tags("Usuários")]
public class CreateUserController(CreateUserUseCase createUser) : ControllerBase
{
    private readonly CreateUserUseCase _createUser = createUser;

    /// <summary>
    /// Cria uma nova conta de usuário.
    /// </summary>
    /// <remarks>
    /// Endpoint público (não exige autenticação). Sujeito a rate limit de 10 requisições/minuto por IP.
    /// </remarks>
    [HttpPost]
    public async Task<ActionResult<CreateUserResult>> Create(CreateUserRequest request, CancellationToken ct)
    {
        var result = await _createUser.ExecuteAsync(request, ct);
        return Created($"/api/users/{result.Id}", result);
    }
}
