using Authentication.Infrastructure;

using Shopping.Infrastructure;

using Users.Infrastructure;

namespace Api.Migrations;

public static class MigrationExtensions
{
    public static async Task RunPendingMigrationsAsync(this WebApplication app)
    {
        if (!app.Configuration.GetValue<bool>("RUN_MIGRATIONS"))
        {
            return;
        }

        await app.Services.MigrateUsersModuleAsync();
        await app.Services.MigrateAuthenticationModuleAsync();
        await app.Services.MigrateShoppingModulesAsync();
    }
}
