using Fenicia.Common.Data.Responses.Auth;

namespace Fenicia.Auth.Domains.Submodule;

public interface ISubmoduleService
{
    Task<List<SubmoduleResponse>> GetByModuleIdAsync(Guid moduleId, CancellationToken ct);
}