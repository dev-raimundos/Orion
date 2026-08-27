using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Authentication.Application.Abstractions;
using Authentication.Application.UseCases.Login;
using Authentication.Application.UseCases.Logout;
using Authentication.Application.UseCases.RefreshToken;
using Authentication.Domain.Abstractions;
using Authentication.Infrastructure.Persistence;
using Authentication.Infrastructure.Security;

namespace Authentication.Infrastructure;

public static class AuthenticationModuleExtensions
{
    public static IServiceCollection AddAuthenticationModule(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DatabaseConnection")
            ?? throw new InvalidOperationException("Connection string 'DatabaseConnection' não configurada.");

        services.AddDbContext<AuthenticationDbContext>(options => options.UseSqlServer(connectionString));

        services.AddHealthChecks()
            .AddDbContextCheck<AuthenticationDbContext>(name: "authentication-db");

        services.AddScoped<ILoginAttemptRepository, LoginAttemptRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<ITokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IRefreshTokenGenerator, RefreshTokenGenerator>();
        services.AddScoped<LoginUseCase>();
        services.AddScoped<RefreshTokenUseCase>();
        services.AddScoped<LogoutUseCase>();

        return services;
    }

    public static async Task MigrateAuthenticationModuleAsync(this IServiceProvider services, CancellationToken ct = default)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AuthenticationDbContext>();
        await context.Database.MigrateAsync(ct);
    }
}
