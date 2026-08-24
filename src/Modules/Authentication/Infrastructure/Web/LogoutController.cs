using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Authentication.Application.UseCases;

namespace Authentication.Infrastructure.Web;

[ApiController]
[Route("api/auth")]
[AllowAnonymous]
[EnableRateLimiting("auth")]
[Tags("Autenticação")]
public class LogoutController(LogoutUseCase logout) : ControllerBase
{
    private readonly LogoutUseCase _logout = logout;

    /// <summary>
    /// Revoga o refresh token informado, encerrando a sessão.
    /// </summary>
    /// <remarks>
    /// Idempotente: chamar com um refresh token inexistente ou já revogado não retorna erro.
    /// O access token em uso continua válido até expirar naturalmente (não é revogável).
    /// </remarks>
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(LogoutRequest request, CancellationToken ct)
    {
        await _logout.ExecuteAsync(request, ct);
        return NoContent();
    }
}
