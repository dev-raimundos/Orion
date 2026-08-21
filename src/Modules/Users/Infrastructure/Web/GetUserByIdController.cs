using Microsoft.AspNetCore.Mvc;
using Users.Application.UseCases;

namespace Users.Infrastructure.Web;

[ApiController]
[Route("api/users")]
public class GetUserByIdController(GetUserByIdUseCase getUserById) : ControllerBase
{
    private readonly GetUserByIdUseCase _getUserById = getUserById;

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserResponse>> GetById(Guid id, CancellationToken ct)
    {
        var result = await _getUserById.ExecuteAsync(new GetUserByIdRequest(id), ct);
        return Ok(result);
    }
}
