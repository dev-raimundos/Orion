using Users.Domain;

namespace Users.Application.UseCase;

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
            throw new InvalidOperationException("Já existe um usuário com este email");

        var passwordHash = _passwordHasher.Hash(request.Password);
        var user = User.Create(request.Name, request.Email, passwordHash);

        await _repository.AddAsync(user, ct);

        return new CreateUserResult(user.Id);
    }
}
