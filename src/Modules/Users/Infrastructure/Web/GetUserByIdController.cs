using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Users.Application.UseCases;

namespace Users.Infrastructure.Web;

[ApiController]
[Route("api/users")]
[Authorize]
public class GetUserByIdController(GetUserByIdUseCase getUserById) : ControllerBase
{
    private readonly GetUserByIdUseCase _getUserById = getUserById;

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserResponse>> GetById(Guid id, CancellationToken ct)
    {
        this.EnsureIsCurrentUser(id);

        var result = await _getUserById.ExecuteAsync(new GetUserByIdRequest(id), ct);
        return Ok(result);
    }
}
