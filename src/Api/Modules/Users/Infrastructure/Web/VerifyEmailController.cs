using Microsoft.AspNetCore.Mvc;
using Api.Modules.Users.Application.UseCases;

namespace Api.Modules.Users.Infrastructure.Web;

[ApiController]
[Route("api/users")]
public class VerifyEmailController(VerifyEmailUseCase verifyEmail) : ControllerBase
{
    private readonly VerifyEmailUseCase _verifyEmail = verifyEmail;

    [HttpPost("{id:guid}/verify-email")]
    public async Task<ActionResult<VerifyEmailResult>> VerifyEmail(Guid id, CancellationToken ct)
    {
        var result = await _verifyEmail.ExecuteAsync(new VerifyEmailRequest(id), ct);
        return Ok(result);
    }
}
