using Fenicia.Common.Database.Converters.Auth;
using Fenicia.Common.Database.Responses.Auth;

namespace Fenicia.Auth.Domains.Submodule;

public class SubmoduleService(ISubmoduleRepository submoduleRepository) : ISubmoduleService
{
    public async Task<List<SubmoduleResponse>> GetByModuleIdAsync(Guid moduleId, CancellationToken cancellationToken)
    {
        var submodules = await submoduleRepository.GetByModuleIdAsync(moduleId, cancellationToken);

        return SubmoduleConverter.Convert(submodules);
    }
}
