using Orion.SharedKernel;

using Users.Domain.Abstractions;

namespace Users.Application.UseCases.ActivateUser;

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
