using Fenicia.Common.Database.Models.Auth;
using Fenicia.Common.Database.Responses;

namespace Fenicia.Auth.Domains.Submodule;

public class SubmoduleService(ISubmoduleRepository submoduleRepository) : ISubmoduleService
{
    public async Task<List<SubmoduleResponse>> GetByModuleIdAsync(Guid moduleId, CancellationToken cancellationToken)
    {
        var submodules = await submoduleRepository.GetByModuleIdAsync(moduleId, cancellationToken);

        return SubmoduleModel.Convert(submodules);
    }
}
