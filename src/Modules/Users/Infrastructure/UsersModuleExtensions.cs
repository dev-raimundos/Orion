using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orion.SharedKernel.Contracts;
using Users.Application.Abstractions;
using Users.Application.UseCases;
using Users.Domain.Abstractions;
using Users.Infrastructure.Persistence;
using Users.Infrastructure.Security;

namespace Users.Infrastructure;

public static class UsersModuleExtensions
{
    public static IServiceCollection AddUsersModule(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DatabaseConnection")
            ?? throw new InvalidOperationException("Connection string 'DatabaseConnection' não configurada.");

        services.AddDbContext<UsersDbContext>(options => options.UseSqlServer(connectionString));

        services.AddHealthChecks()
            .AddDbContextCheck<UsersDbContext>(name: "users-db");

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IUserCredentialsChecker, UserCredentialsChecker>();

        services.AddScoped<CreateUserUseCase>();
        services.AddScoped<GetUserByIdUseCase>();
        services.AddScoped<RenameUserUseCase>();
        services.AddScoped<ChangePasswordUseCase>();
        services.AddScoped<VerifyEmailUseCase>();
        services.AddScoped<ActivateUserUseCase>();
        services.AddScoped<DeactivateUserUseCase>();

        return services;
    }

    public static async Task MigrateUsersModuleAsync(this IServiceProvider services, CancellationToken ct = default)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<UsersDbContext>();
        await context.Database.MigrateAsync(ct);
    }
}
