using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Users.Application.UseCases;

namespace Users.Infrastructure.Web;

[ApiController]
[Route("api/users")]
[AllowAnonymous]
[EnableRateLimiting("auth")]
public class CreateUserController(CreateUserUseCase createUser) : ControllerBase
{
    private readonly CreateUserUseCase _createUser = createUser;

    [HttpPost]
    public async Task<ActionResult<CreateUserResult>> Create(CreateUserRequest request, CancellationToken ct)
    {
        var result = await _createUser.ExecuteAsync(request, ct);
        return Created($"/api/users/{result.Id}", result);
    }
}
