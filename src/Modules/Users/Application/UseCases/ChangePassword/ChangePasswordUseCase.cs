using Orion.SharedKernel;
using Users.Application.Abstractions;
using Users.Domain.Abstractions;

namespace Users.Application.UseCases.ChangePassword;

public class ChangePasswordUseCase(IUserRepository repository, IPasswordHasher passwordHasher)
{
    private readonly IUserRepository _repository = repository;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;

    public async Task<ChangePasswordOutput> ExecuteAsync(ChangePasswordInput request, CancellationToken ct)
    {
        var user = await _repository.GetByIdAsync(request.UserId, ct)
            ?? throw new AppNotFoundException($"Usuário '{request.UserId}' não encontrado.");

        if (!_passwordHasher.Verify(request.CurrentPassword, user.PasswordHash))
            throw new AppUnauthorizedException("Senha atual inválida.");

        var newPasswordHash = _passwordHasher.Hash(request.NewPassword);
        user.ChangePassword(newPasswordHash);

        await _repository.UpdateAsync(user, ct);

        return new ChangePasswordOutput(user.Id);
    }
}
