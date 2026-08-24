using Microsoft.AspNetCore.HttpOverrides;

namespace Api.Security;

public static class ForwardedHeadersExtensions
{
    // Em produção a app roda atrás de um proxy que termina o TLS (Cloudflare
    // Tunnel/Dokploy) e encaminha pra dentro em HTTP puro. Sem isso, a app não
    // sabe que a requisição original veio por HTTPS. Limpar
    // KnownIPNetworks/KnownProxies é seguro aqui porque o container não expõe
    // porta pro host (ver "expose" no docker-compose.yml) — só o proxy, na
    // mesma rede Docker, consegue falar com ele.
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
