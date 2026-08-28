namespace Users.Application.UseCases.GetUserById;

public sealed record UserOutput(
    Guid Id,
    string Name,
    string Email,
    bool Active,
    bool EmailVerified,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    DateTimeOffset? LastLoginAt);
