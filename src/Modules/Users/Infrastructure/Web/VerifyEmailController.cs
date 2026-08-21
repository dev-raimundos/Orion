using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Users.Application.UseCases;

namespace Users.Infrastructure.Web;

[ApiController]
[Route("api/users")]
[Authorize]
public class VerifyEmailController(VerifyEmailUseCase verifyEmail) : ControllerBase
{
    private readonly VerifyEmailUseCase _verifyEmail = verifyEmail;

    [HttpPost("{id:guid}/verify-email")]
    public async Task<ActionResult<VerifyEmailResult>> VerifyEmail(Guid id, CancellationToken ct)
    {
        this.EnsureIsCurrentUser(id);

        var result = await _verifyEmail.ExecuteAsync(new VerifyEmailRequest(id), ct);
        return Ok(result);
    }
}
