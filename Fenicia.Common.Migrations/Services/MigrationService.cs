using Fenicia.Common.Api;
using Fenicia.Common.Database.Contexts;
using Fenicia.Common.Database.Responses;
using Fenicia.Common.Enums;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Common.Migrations.Services;

public class MigrationService : IMigrationService
{
    public async Task RunMigrationsAsync(List<ModuleResponse> modules, Guid companyId, CancellationToken cancellationToken)
    {
        foreach (var module in modules)
        {
            var connectionString = GetConnectionString(module.Type, companyId);
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new DbUpdateException($"No connection string for {module.Type}");
            }

            var (dbContextType, migrationsAssembly) = GetModuleDbInfo(module.Type);

            var optionsBuilderType = typeof(DbContextOptionsBuilder<>).MakeGenericType(dbContextType);
            var optionsBuilder = (DbContextOptionsBuilder)Activator.CreateInstance(optionsBuilderType)!;

            optionsBuilder.UseNpgsql(connectionString, npgsql =>
                                         npgsql.MigrationsAssembly(migrationsAssembly));

            var options = optionsBuilder.Options;

            await using var context = (DbContext)Activator.CreateInstance(dbContextType, options)!;

            await context.Database.MigrateAsync(cancellationToken);
        }
    }

    private static string? GetConnectionString(ModuleType type, Guid companyId)
    {
        if (type != ModuleType.Basic)
        {
            return string.Empty;
        }

        var connectionString = AppSettingsReader.GetConnectionString("BasicConnection");

        return connectionString?.Replace("{tenant}", companyId.ToString()) ?? null;
    }

    private static (Type dbContextType, string migrationsAssembly) GetModuleDbInfo(ModuleType type)
    {
        return type switch
               {
                   ModuleType.Basic => (typeof(BasicContext), "Fenicia.Modules.Basic"),
                   _ => throw new NotSupportedException($"Module {type} not supported")
               };
    }
}
