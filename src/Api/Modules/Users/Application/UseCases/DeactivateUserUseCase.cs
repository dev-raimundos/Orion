using Api.Common;
using Api.Modules.Users.Domain.Abstractions;

namespace Api.Modules.Users.Application.UseCases;

public sealed record DeactivateUserRequest(Guid UserId);
public sealed record DeactivateUserResult(Guid Id, bool Active);

public class DeactivateUserUseCase(IUserRepository repository)
{
    private readonly IUserRepository _repository = repository;

    public async Task<DeactivateUserResult> ExecuteAsync(DeactivateUserRequest request, CancellationToken ct)
    {
        var user = await _repository.GetByIdAsync(request.UserId, ct)
            ?? throw new AppNotFoundException($"Usuário '{request.UserId}' não encontrado.");

        user.Deactivate();

        await _repository.UpdateAsync(user, ct);

        return new DeactivateUserResult(user.Id, user.Active);
    }
}
