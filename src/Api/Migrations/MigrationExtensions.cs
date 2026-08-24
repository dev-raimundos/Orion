using Authentication.Infrastructure;
using Users.Infrastructure;

namespace Api.Migrations;

public static class MigrationExtensions
{
    // Aplica as migrations pendentes de Users e Authentication no startup.
    // Fica "false" por padrão (config "RUN_MIGRATIONS") — liga "true" só no
    // deploy em que você quer mesmo aplicar migration.
    public static async Task RunPendingMigrationsAsync(this WebApplication app)
    {
        if (!app.Configuration.GetValue<bool>("RUN_MIGRATIONS"))
        {
            return;
        }

        await app.Services.MigrateUsersModuleAsync();
        await app.Services.MigrateAuthenticationModuleAsync();
    }
}
