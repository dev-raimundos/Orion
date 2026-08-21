using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Authentication.Application.UseCases;

namespace Authentication.Infrastructure.Web;

[ApiController]
[Route("api/auth")]
[AllowAnonymous]
public class LogoutController(LogoutUseCase logout) : ControllerBase
{
    private readonly LogoutUseCase _logout = logout;

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(LogoutRequest request, CancellationToken ct)
    {
        await _logout.ExecuteAsync(request, ct);
        return NoContent();
    }
}
