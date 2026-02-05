using Fenicia.Common.Database.Abstracts;
using Fenicia.Common.Database.Models.Auth;

namespace Fenicia.Auth.Domains.Submodule;

public interface ISubmoduleRepository: IBaseRepository<SubmoduleModel>
{
    Task<List<SubmoduleModel>> GetByModuleIdAsync(Guid moduleId, CancellationToken cancellationToken);
}
