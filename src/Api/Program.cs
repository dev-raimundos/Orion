using System.Text;
using Api.Exceptions;
using Authentication.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Users.Infrastructure;

namespace Api;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();

        builder.Services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer((document, context, cancellationToken) =>
            {
                var components = document.Components ??= new OpenApiComponents();
                components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
                components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Name = "Authorization"
                };

                return Task.CompletedTask;
            });
        });

        builder.Services.AddUsersModule(builder.Configuration);
        builder.Services.AddAuthenticationModule(builder.Configuration);

        var jwtSection = builder.Configuration.GetSection("Jwt");
        var signingKey = jwtSection["SigningKey"]
            ?? throw new InvalidOperationException("Configuração 'Jwt:SigningKey' não configurada.");

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.MapInboundClaims = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtSection["Issuer"],
                    ValidateAudience = true,
                    ValidAudience = jwtSection["Audience"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
                    ValidateLifetime = true
                };
            });

        builder.Services.AddAuthorization();

        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddProblemDetails();

        // Em produção a app roda atrás do proxy do Dokploy/Traefik, que termina o
        // TLS e encaminha pra dentro em HTTP puro. Sem isso, a app não sabe que a
        // requisição original veio por HTTPS (afeta UseHttpsRedirection, cookies
        // Secure, e qualquer URL absoluta que a app venha a gerar). Limpar
        // KnownNetworks/KnownProxies é seguro aqui porque o container não expõe
        // porta pro host (ver "expose" no docker-compose.yml) — só o proxy, na
        // mesma rede Docker, consegue falar com ele.
        builder.Services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            options.KnownIPNetworks.Clear();
            options.KnownProxies.Clear();
        });

        var app = builder.Build();

        if (builder.Configuration.GetValue<bool>("RUN_MIGRATIONS"))
        {
            await app.Services.MigrateUsersModuleAsync();
            await app.Services.MigrateAuthenticationModuleAsync();
        }

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/openapi/v1.json", "Orion API v1");
            });
        }

        app.UseExceptionHandler();

        app.UseForwardedHeaders();

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        app.MapHealthChecks("/health");

        await app.RunAsync();
    }
}
