using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Authentication.Application.Abstractions;
using Authentication.Application.UseCases;
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

        services.AddScoped<ILoginAttemptRepository, LoginAttemptRepository>();
        services.AddScoped<ITokenGenerator, JwtTokenGenerator>();
        services.AddScoped<LoginUseCase>();

        return services;
    }
}
