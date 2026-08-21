using Microsoft.EntityFrameworkCore;
using Api.Modules.Users.Application.Abstractions;
using Api.Modules.Users.Application.UseCases;
using Api.Modules.Users.Domain.Abstractions;
using Api.Modules.Users.Infrastructure.Persistence;
using Api.Modules.Users.Infrastructure.Security;

namespace Api.Modules.Users.Infrastructure;

public static class UsersModuleExtensions
{
    public static IServiceCollection AddUsersModule(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DatabaseConnection")
            ?? throw new InvalidOperationException("Connection string 'DatabaseConnection' não configurada.");

        services.AddDbContext<UsersDbContext>(options => options.UseSqlServer(connectionString));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();

        services.AddScoped<CreateUserUseCase>();
        services.AddScoped<GetUserByIdUseCase>();
        services.AddScoped<RenameUserUseCase>();
        services.AddScoped<ChangePasswordUseCase>();
        services.AddScoped<VerifyEmailUseCase>();
        services.AddScoped<ActivateUserUseCase>();
        services.AddScoped<DeactivateUserUseCase>();

        return services;
    }
}
