using Microsoft.AspNetCore.Mvc;
using Api.Modules.Users.Application.UseCases;

namespace Api.Modules.Users.Infrastructure.Web;

[ApiController]
[Route("api/users")]
public class DeactivateUserController(DeactivateUserUseCase deactivateUser) : ControllerBase
{
    private readonly DeactivateUserUseCase _deactivateUser = deactivateUser;

    [HttpPost("{id:guid}/deactivate")]
    public async Task<ActionResult<DeactivateUserResult>> Deactivate(Guid id, CancellationToken ct)
    {
        var result = await _deactivateUser.ExecuteAsync(new DeactivateUserRequest(id), ct);
        return Ok(result);
    }
}
