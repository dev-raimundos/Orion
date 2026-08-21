using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Users.Application.UseCases;

namespace Users.Infrastructure.Web;

[ApiController]
[Route("api/users")]
[Authorize]
public class RenameUserController(RenameUserUseCase renameUser) : ControllerBase
{
    private readonly RenameUserUseCase _renameUser = renameUser;

    [HttpPut("{id:guid}/name")]
    public async Task<ActionResult<RenameUserResult>> Rename(Guid id, [FromBody] string newName, CancellationToken ct)
    {
        this.EnsureIsCurrentUser(id);

        var result = await _renameUser.ExecuteAsync(new RenameUserRequest(id, newName), ct);
        return Ok(result);
    }
}
