namespace Fenicia.Common.Migrations.Services;

using Enums;

public interface IMigrationService
{
    Task RunMigrationsAsync(Guid companyId, List<ModuleType> moduleTypes, CancellationToken cancellationToken);
}
