namespace Fenicia.Common.Migrations.Services;

using Fenicia.Common.Database.Contexts;
using Fenicia.Common.Enums;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

public class MigrationService : IMigrationService
{
    public async Task RunMigrationsAsync(Guid companyId, List<ModuleType> moduleTypes, CancellationToken cancellationToken)
    {
        foreach (var module in moduleTypes.Where(module => module == ModuleType.Basic))
        {
            var (dbContextType, migrationsAssembly, connectionStringName) = MigrationService.GetModuleDbInfo(module);

            // Get the real connection string from appsettings.json
            var rawConnectionString = Fenicia.Common.API.AppSettingsReader.GetConnectionString(connectionStringName);
            if (string.IsNullOrWhiteSpace(rawConnectionString))
            {
                throw new InvalidOperationException($"Connection string '{connectionStringName}' not found in appsettings.json");
            }

            // Replace {tenant} with the companyID
            var connectionString = rawConnectionString.Replace("{tenant}", companyId.ToString());

            var optionsBuilderType = typeof(DbContextOptionsBuilder<>).MakeGenericType(dbContextType);
            var optionsBuilder = (DbContextOptionsBuilder)Activator.CreateInstance(optionsBuilderType)!;

            optionsBuilder.UseNpgsql(connectionString, npgsql =>
                                         npgsql.MigrationsAssembly(migrationsAssembly))
                                         .ConfigureWarnings(x => x.Ignore(RelationalEventId.PendingModelChangesWarning));

            var options = optionsBuilder.Options;

            await using var context = (DbContext)Activator.CreateInstance(dbContextType, options)!;

            await context.Database.MigrateAsync(cancellationToken);
        }
    }

    private static (Type dbContextType, string migrationsAssembly, string connectionString) GetModuleDbInfo(ModuleType type)
    {
        return type switch
        {
            ModuleType.Basic => (typeof(BasicContext), "Fenicia.Module.Basic", "Basic"),
            ModuleType.SocialNetwork => (typeof(SocialNetworkContext), "Fenicia.Module.SocialNetwork", "SocialNetwork"),
            ModuleType.Project => (typeof(ProjectContext), "Fenicia.Module.Projects", "Projects"),
            ModuleType.PerformanceEvaluation => (typeof(PerformanceEvaluationContext), "Fenicia.Module.PerformanceEvaluation", "PerformanceEvaluation"),
            ModuleType.Accounting => (typeof(AccountingContext), "Fenicia.Module.Accounting", "Accounting"),
            ModuleType.Hr => (typeof(HrContext), "Fenicia.Module.HR", "HR"),
            ModuleType.Pos => (typeof(PosContext), "Fenicia.Module.POS", "Pos"),
            ModuleType.Contracts => (typeof(ContractsContext), "Fenicia.Module.Contracts", "Contracts"),
            ModuleType.Ecommerce => (typeof(EcommerceContext), "Fenicia.Module.Ecommerce", "EcommerceSupport"),
            ModuleType.CustomerSupport => (typeof(CustomerSupportContext), "Fenicia.Module.CustomerSupport", "CustomerSupport"),
            ModuleType.Plus => (typeof(PlusContext), "Fenicia.Module.Plus", "Plus"),
            _ => throw new NotSupportedException($"Module {type} not supported")
        };
    }
}
