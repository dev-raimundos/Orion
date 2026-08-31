using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Shopping.Infrastructure.Persistence;

namespace Shopping.Infrastructure;

public static class ShoppingModuleExtensions
{
    public static IServiceCollection AddShoppingModule(this IServiceCollection services, IConfiguration configuration)
    {
        string? connectionString = configuration.GetConnectionString("DatabaseConnection")
            ?? throw new InvalidOperationException("Connection string not found");

        services.AddDbContext<ShoppingDbContext>(options => options.UseSqlServer(connectionString));

        return services;
    }

    //TODO: registrar método no program.cs
    public static async Task MigrateShoppingModulesAsync(this IServiceProvider services, CancellationToken ct = default)
    {
        AsyncServiceScope scope = services.CreateAsyncScope();
        ShoppingDbContext context = scope.ServiceProvider.GetRequiredService<ShoppingDbContext>();
        await context.Database.MigrateAsync(ct);
    }
}
