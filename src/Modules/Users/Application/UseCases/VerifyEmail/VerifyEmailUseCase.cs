using Orion.SharedKernel;
using Users.Domain.Abstractions;

namespace Users.Application.UseCases.VerifyEmail;

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
