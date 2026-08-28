using Orion.SharedKernel;
using Users.Domain.Abstractions;

namespace Users.Application.UseCases.GetUserById;

public class GetUserByIdUseCase(IUserRepository repository)
{
    private readonly IUserRepository _repository = repository;

    public async Task<UserOutput> ExecuteAsync(GetUserByIdInput request, CancellationToken ct)
    {
        var user = await _repository.GetByIdAsync(request.UserId, ct)
            ?? throw new AppNotFoundException($"Usuário '{request.UserId}' não encontrado.");

        return new UserOutput(
            user.Id,
            user.Name,
            user.Email,
            user.Active,
            user.EmailVerified,
            user.CreatedAt,
            user.UpdatedAt,
            user.LastLoginAt);
    }
}
