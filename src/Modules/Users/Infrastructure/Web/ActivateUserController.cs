using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Users.Application.UseCases;

namespace Users.Infrastructure.Web;

[ApiController]
[Route("api/users")]
[Authorize]
public class ActivateUserController(ActivateUserUseCase activateUser) : ControllerBase
{
    private readonly ActivateUserUseCase _activateUser = activateUser;

    [HttpPost("{id:guid}/activate")]
    public async Task<ActionResult<ActivateUserResult>> Activate(Guid id, CancellationToken ct)
    {
        this.EnsureIsCurrentUser(id);

        var result = await _activateUser.ExecuteAsync(new ActivateUserRequest(id), ct);
        return Ok(result);
    }
}
