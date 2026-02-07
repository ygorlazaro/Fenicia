using Fenicia.Common.Data.Abstracts;
using Fenicia.Common.Data.Models.Auth;

namespace Fenicia.Auth.Domains.Submodule;

public interface ISubmoduleRepository : IBaseRepository<SubmoduleModel>
{
    Task<List<SubmoduleModel>> GetByModuleIdAsync(Guid moduleId, CancellationToken ct);
}