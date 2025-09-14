namespace Fenicia.Common.Migrations.Services;

using Fenicia.Common.Database.Responses;

public interface IMigrationService
{
    Task RunMigrationsAsync(List<ModuleResponse> modules, Guid companyId, CancellationToken cancellationToken);
}
