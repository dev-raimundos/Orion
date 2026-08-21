using Microsoft.AspNetCore.Mvc;
using Api.Modules.Users.Application.UseCases;

namespace Api.Modules.Users.Infrastructure.Web;

[ApiController]
[Route("api/users")]
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
