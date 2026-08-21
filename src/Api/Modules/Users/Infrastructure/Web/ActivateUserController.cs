using Microsoft.AspNetCore.Mvc;
using Api.Modules.Users.Application.UseCases;

namespace Api.Modules.Users.Infrastructure.Web;

[ApiController]
[Route("api/users")]
public class ActivateUserController(ActivateUserUseCase activateUser) : ControllerBase
{
    private readonly ActivateUserUseCase _activateUser = activateUser;

    [HttpPost("{id:guid}/activate")]
    public async Task<ActionResult<ActivateUserResult>> Activate(Guid id, CancellationToken ct)
    {
        var result = await _activateUser.ExecuteAsync(new ActivateUserRequest(id), ct);
        return Ok(result);
    }
}
