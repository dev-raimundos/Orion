using Api.Exceptions;
using Api.Migrations;
using Api.OpenApi;
using Api.Security;
using Authentication.Infrastructure;
using Users.Infrastructure;

namespace Api;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddOpenApiWithBearerAuth();

        builder.Services.AddUsersModule(builder.Configuration);
        builder.Services.AddAuthenticationModule(builder.Configuration);

        builder.Services.AddJwtAuthentication(builder.Configuration);
        builder.Services.AddAuthorization();

        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddProblemDetails();

        builder.Services.AddReverseProxyForwardedHeaders();

        var app = builder.Build();

        await app.RunPendingMigrationsAsync();

        app.UseExceptionHandler();

        app.UseForwardedHeaders();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/openapi/v1.json", "Orion API v1");
            });
            app.MapGet("/", () => Results.Redirect("/swagger"));
            app.UseHttpsRedirection();
        }

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        app.MapHealthChecks("/health");

        await app.RunAsync();
    }
}
