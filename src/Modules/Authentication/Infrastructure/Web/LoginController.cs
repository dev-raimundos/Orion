using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Authentication.Application.UseCases;

namespace Authentication.Infrastructure.Web;

[ApiController]
[Route("api/auth")]
[AllowAnonymous]
public class LoginController(LoginUseCase login) : ControllerBase
{
    private readonly LoginUseCase _login = login;

    [HttpPost("login")]
    public async Task<ActionResult<LoginResult>> Login(LoginRequest request, CancellationToken ct)
    {
        var result = await _login.ExecuteAsync(request, ct);
        return Ok(result);
    }
}
