using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Authentication.Application.UseCases.Login;

namespace Authentication.Infrastructure.Web;

[ApiController]
[Route("api/auth")]
[AllowAnonymous]
[EnableRateLimiting("auth")]
[Tags("Autenticação")]
public class LoginController(LoginUseCase login) : ControllerBase
{
    private readonly LoginUseCase _login = login;

    /// <summary>
    /// Autentica um usuário com email e senha, retornando um access token (JWT, curta duração) e um refresh token (longa duração).
    /// </summary>
    /// <remarks>
    /// Após 5 tentativas com senha incorreta em 15 minutos, a conta fica temporariamente bloqueada (423), mesmo com a senha certa.
    /// Também sujeito a rate limit de 10 requisições/minuto por IP.
    /// </remarks>
    [HttpPost("login")]
    public async Task<ActionResult<LoginResult>> Login(LoginRequest request, CancellationToken ct)
    {
        var result = await _login.ExecuteAsync(request, ct);
        return Ok(result);
    }
}

