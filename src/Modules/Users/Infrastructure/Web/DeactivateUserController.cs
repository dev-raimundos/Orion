using Microsoft.AspNetCore.Mvc;
using Users.Application.UseCases;

namespace Users.Infrastructure.Web;

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
