using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Users.Application.UseCases.VerifyEmail;

namespace Users.Infrastructure.Web;

[ApiController]
[Route("api/users")]
[Authorize]
[Tags("Usuários")]
public class VerifyEmailController(VerifyEmailUseCase verifyEmail) : ControllerBase
{
    private readonly VerifyEmailUseCase _verifyEmail = verifyEmail;

    /// <summary>
    /// Marca o email do usuário como verificado.
    /// </summary>
    /// <remarks>
    /// Requer um access token válido cujo Id (claim "sub") seja igual ao <paramref name="id"/> da rota.
    /// </remarks>
    [HttpPost("{id:guid}/verify-email")]
    public async Task<ActionResult<VerifyEmailResult>> VerifyEmail(Guid id, CancellationToken ct)
    {
        this.EnsureIsCurrentUser(id);

        var result = await _verifyEmail.ExecuteAsync(new VerifyEmailRequest(id), ct);
        return Ok(result);
    }
}
