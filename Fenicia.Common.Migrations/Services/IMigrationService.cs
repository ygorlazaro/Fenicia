namespace Fenicia.Common.Migrations.Services;

using Fenicia.Common.Enums;

public interface IMigrationService
{
    Task RunMigrationsAsync(Guid companyId, List<ModuleType> moduleTypes, CancellationToken cancellationToken);
}
