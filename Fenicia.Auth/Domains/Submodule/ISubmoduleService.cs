using Fenicia.Common.Database.Responses;

namespace Fenicia.Auth.Domains.Submodule;

public interface ISubmoduleService
{
    Task<List<SubmoduleResponse>> GetByModuleIdAsync(Guid moduleId, CancellationToken cancellationToken);
}
