using Orion.SharedKernel;
using Users.Domain.Abstractions;

namespace Users.Application.UseCases.DeactivateUser;

public class DeactivateUserUseCase(IUserRepository repository)
{
    private readonly IUserRepository _repository = repository;

    public async Task<DeactivateUserOutput> ExecuteAsync(DeactivateUserInput request, CancellationToken ct)
    {
        var user = await _repository.GetByIdAsync(request.UserId, ct)
            ?? throw new AppNotFoundException($"Usuário '{request.UserId}' não encontrado.");

        user.Deactivate();

        await _repository.UpdateAsync(user, ct);

        return new DeactivateUserOutput(user.Id, user.Active);
    }
}
