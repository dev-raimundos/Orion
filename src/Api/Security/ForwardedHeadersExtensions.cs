using Microsoft.AspNetCore.HttpOverrides;

namespace Api.Security;

public static class ForwardedHeadersExtensions
{
    public static IServiceCollection AddReverseProxyForwardedHeaders(this IServiceCollection services)
    {
        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            options.KnownIPNetworks.Clear();
            options.KnownProxies.Clear();
        });

        return services;
    }
}
