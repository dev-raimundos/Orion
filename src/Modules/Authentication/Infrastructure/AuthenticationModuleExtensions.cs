using Microsoft.Extensions.DependencyInjection;
using Authentication.Application.Abstractions;
using Authentication.Application.UseCases;
using Authentication.Infrastructure.Security;

namespace Authentication.Infrastructure;

public static class AuthenticationModuleExtensions
{
    public static IServiceCollection AddAuthenticationModule(this IServiceCollection services)
    {
        services.AddScoped<ITokenGenerator, JwtTokenGenerator>();
        services.AddScoped<LoginUseCase>();

        return services;
    }
}
