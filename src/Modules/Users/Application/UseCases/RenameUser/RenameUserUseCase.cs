using Orion.SharedKernel;
using Users.Domain.Abstractions;

namespace Users.Application.UseCases.RenameUser;

public class RenameUserUseCase(IUserRepository repository)
{
    private readonly IUserRepository _repository = repository;

    public async Task<RenameUserResult> ExecuteAsync(RenameUserRequest request, CancellationToken ct)
    {
        var user = await _repository.GetByIdAsync(request.UserId, ct)
            ?? throw new AppNotFoundException($"Usuário '{request.UserId}' não encontrado.");

        user.Rename(request.NewName);

        await _repository.UpdateAsync(user, ct);

        return new RenameUserResult(user.Id, user.Name);
    }
}
