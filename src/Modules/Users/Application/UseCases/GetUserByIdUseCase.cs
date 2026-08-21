using Orion.SharedKernel;
using Users.Domain.Abstractions;

namespace Users.Application.UseCases;

public sealed record GetUserByIdRequest(Guid UserId);

public sealed record UserResponse(
    Guid Id,
    string Name,
    string Email,
    bool Active,
    bool EmailVerified,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    DateTimeOffset? LastLoginAt);

public class GetUserByIdUseCase(IUserRepository repository)
{
    private readonly IUserRepository _repository = repository;

    public async Task<UserResponse> ExecuteAsync(GetUserByIdRequest request, CancellationToken ct)
    {
        var user = await _repository.GetByIdAsync(request.UserId, ct)
            ?? throw new AppNotFoundException($"Usuário '{request.UserId}' não encontrado.");

        return new UserResponse(
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
