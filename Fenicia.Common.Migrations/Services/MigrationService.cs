using Fenicia.Common.API;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Enums.Auth;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Fenicia.Common.Migrations.Services;

public class MigrationService : IMigrationService
{
    public async Task RunMigrationsAsync(Guid companyId, List<ModuleType> moduleTypes, CancellationToken ct)
    {
        foreach (var module in moduleTypes)
        {
            var (dbContextType, migrationsAssembly, connectionStringName) = GetModuleDbInfo(module);
            var rawConnectionString = AppSettingsReader.GetConnectionString(connectionStringName);

            if (string.IsNullOrWhiteSpace(rawConnectionString))
            {
                throw new InvalidOperationException(
                    $"Connection string '{connectionStringName}' not found in appsettings.json");
            }

            var connectionString = rawConnectionString.Replace("{tenant}", companyId.ToString());
            var optionsBuilderType = typeof(DbContextOptionsBuilder<>).MakeGenericType(dbContextType);
            var optionsBuilder = (DbContextOptionsBuilder)Activator.CreateInstance(optionsBuilderType)!;

            optionsBuilder.UseNpgsql(connectionString, npgsql =>
                    npgsql.MigrationsAssembly(migrationsAssembly))
                .ConfigureWarnings(x => x.Ignore(RelationalEventId.PendingModelChangesWarning));

            var options = optionsBuilder.Options;

            await using var context = (DbContext)Activator.CreateInstance(dbContextType, options)!;
            await context.Database.MigrateAsync(ct);
        }
    }

    private static (Type dbContextType, string migrationsAssembly, string connectionString) GetModuleDbInfo(
        ModuleType type)
    {
        return type switch
        {
            ModuleType.Basic => (typeof(BasicContext), "Fenicia.Module.Basic", "Basic"),
            ModuleType.SocialNetwork => (typeof(SocialNetworkContext), "Fenicia.Module.SocialNetwork", "SocialNetwork"),
            ModuleType.Project => (typeof(ProjectContext), "Fenicia.Module.Projects", "Projects"),
            ModuleType.PerformanceEvaluation => (typeof(PerformanceEvaluationContext),
                "Fenicia.Module.PerformanceEvaluation", "PerformanceEvaluation"),
            ModuleType.Accounting => (typeof(AccountingContext), "Fenicia.Module.Accounting", "Accounting"),
            ModuleType.Hr => (typeof(HrContext), "Fenicia.Module.HR", "HR"),
            ModuleType.Pos => (typeof(PosContext), "Fenicia.Module.POS", "Pos"),
            ModuleType.Contracts => (typeof(ContractsContext), "Fenicia.Module.Contracts", "Contracts"),
            ModuleType.Ecommerce => (typeof(EcommerceContext), "Fenicia.Module.Ecommerce", "EcommerceSupport"),
            ModuleType.CustomerSupport => (typeof(CustomerSupportContext), "Fenicia.Module.CustomerSupport",
                "CustomerSupport"),
            ModuleType.Plus => (typeof(PlusContext), "Fenicia.Module.Plus", "Plus"),
            _ => throw new NotSupportedException($"Module {type} not supported")
        };
    }
}
