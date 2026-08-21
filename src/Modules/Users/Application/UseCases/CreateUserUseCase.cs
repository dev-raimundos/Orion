using Orion.Application;
using Users.Application.Abstractions;
using Users.Domain;
using Users.Domain.Abstractions;

namespace Users.Application.UseCases;

public sealed record CreateUserRequest(string Name, string Email, string Password);
public sealed record CreateUserResult(Guid Id);

public class CreateUserUseCase(IUserRepository repository, IPasswordHasher passwordHasher)
{
    private readonly IUserRepository _repository = repository;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;

    public async Task<CreateUserResult> ExecuteAsync(CreateUserRequest request, CancellationToken ct)
    {
        var existing = await _repository.GetByEmailAsync(request.Email, ct);

        if (existing is not null)
            throw new AppConflictException($"Já existe um usuário com o email '{request.Email}'.");

        var passwordHash = _passwordHasher.Hash(request.Password);
        var user = User.Create(request.Name, request.Email, passwordHash);

        await _repository.AddAsync(user, ct);

        return new CreateUserResult(user.Id);
    }
}