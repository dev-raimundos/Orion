using Microsoft.AspNetCore.Mvc;
using Api.Modules.Users.Application.UseCases;

namespace Api.Modules.Users.Infrastructure.Web;

[ApiController]
[Route("api/users")]
public class RenameUserController(RenameUserUseCase renameUser) : ControllerBase
{
    private readonly RenameUserUseCase _renameUser = renameUser;

    [HttpPut("{id:guid}/name")]
    public async Task<ActionResult<RenameUserResult>> Rename(Guid id, [FromBody] string newName, CancellationToken ct)
    {
        var result = await _renameUser.ExecuteAsync(new RenameUserRequest(id, newName), ct);
        return Ok(result);
    }
}
