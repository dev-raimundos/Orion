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
public class RefreshTokenController(RefreshTokenUseCase refreshToken) : ControllerBase
{
    private readonly RefreshTokenUseCase _refreshToken = refreshToken;

    /// <summary>
    /// Troca um refresh token válido por um novo access token, sem precisar de email/senha de novo.
    /// </summary>
    /// <remarks>
    /// Rotação: o refresh token usado é revogado e um novo é devolvido junto — o cliente deve substituir
    /// o refresh token antigo pelo novo. Reusar um refresh token já revogado retorna 401.
    /// </remarks>
    [HttpPost("refresh")]
    public async Task<ActionResult<RefreshTokenResult>> Refresh(RefreshTokenRequest request, CancellationToken ct)
    {
        var result = await _refreshToken.ExecuteAsync(request, ct);
        return Ok(result);
    }
}
