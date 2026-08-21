using Api.Common;
using Api.Modules.Users.Domain.Abstractions;

namespace Api.Modules.Users.Application.UseCases;

public sealed record ActivateUserRequest(Guid UserId);
public sealed record ActivateUserResult(Guid Id, bool Active);

public class ActivateUserUseCase(IUserRepository repository)
{
    private readonly IUserRepository _repository = repository;

    public async Task<ActivateUserResult> ExecuteAsync(ActivateUserRequest request, CancellationToken ct)
    {
        var user = await _repository.GetByIdAsync(request.UserId, ct)
            ?? throw new AppNotFoundException($"Usuário '{request.UserId}' não encontrado.");

        user.Activate();

        await _repository.UpdateAsync(user, ct);

        return new ActivateUserResult(user.Id, user.Active);
    }
}
