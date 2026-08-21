using Api.Common;
using Api.Modules.Users.Domain.Abstractions;

namespace Api.Modules.Users.Application.UseCases;

public sealed record VerifyEmailRequest(Guid UserId);
public sealed record VerifyEmailResult(Guid Id, bool EmailVerified);

public class VerifyEmailUseCase(IUserRepository repository)
{
    private readonly IUserRepository _repository = repository;

    public async Task<VerifyEmailResult> ExecuteAsync(VerifyEmailRequest request, CancellationToken ct)
    {
        var user = await _repository.GetByIdAsync(request.UserId, ct)
            ?? throw new AppNotFoundException($"Usuário '{request.UserId}' não encontrado.");

        user.VerifyEmail();

        await _repository.UpdateAsync(user, ct);

        return new VerifyEmailResult(user.Id, user.EmailVerified);
    }
}
