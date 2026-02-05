using Fenicia.Common.Database.Models.Auth;

namespace Fenicia.Auth.Domains.Submodule;

public interface ISubmoduleRepository
{
    Task<List<SubmoduleModel>> GetByModuleIdAsync(Guid moduleId, CancellationToken cancellationToken);
}
