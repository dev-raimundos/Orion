namespace Users.Application.UseCases.CreateUser;

public sealed record CreateUserRequest(string Name, string Email, string Password);
