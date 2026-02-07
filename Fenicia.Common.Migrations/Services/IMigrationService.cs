using Fenicia.Common.Enums.Auth;

namespace Fenicia.Common.Migrations.Services;

public interface IMigrationService
{
    Task RunMigrationsAsync(Guid companyId, List<ModuleType> moduleTypes, CancellationToken ct);
}