using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Users.Application.UseCases;

namespace Users.Infrastructure.Web;

[ApiController]
[Route("api/users")]
[Authorize]
public class DeactivateUserController(DeactivateUserUseCase deactivateUser) : ControllerBase
{
    private readonly DeactivateUserUseCase _deactivateUser = deactivateUser;

    [HttpPost("{id:guid}/deactivate")]
    public async Task<ActionResult<DeactivateUserResult>> Deactivate(Guid id, CancellationToken ct)
    {
        this.EnsureIsCurrentUser(id);

        var result = await _deactivateUser.ExecuteAsync(new DeactivateUserRequest(id), ct);
        return Ok(result);
    }
}
