using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Authentication.Application.UseCases;

namespace Authentication.Infrastructure.Web;

[ApiController]
[Route("api/auth")]
[AllowAnonymous]
public class RefreshTokenController(RefreshTokenUseCase refreshToken) : ControllerBase
{
    private readonly RefreshTokenUseCase _refreshToken = refreshToken;

    [HttpPost("refresh")]
    public async Task<ActionResult<RefreshTokenResult>> Refresh(RefreshTokenRequest request, CancellationToken ct)
    {
        var result = await _refreshToken.ExecuteAsync(request, ct);
        return Ok(result);
    }
}
