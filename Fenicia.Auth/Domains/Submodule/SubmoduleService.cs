using Fenicia.Common.Data.Converters.Auth;
using Fenicia.Common.Data.Responses.Auth;

namespace Fenicia.Auth.Domains.Submodule;

public class SubmoduleService(ISubmoduleRepository submoduleRepository) : ISubmoduleService
{
    public async Task<List<SubmoduleResponse>> GetByModuleIdAsync(Guid moduleId, CancellationToken cancellationToken)
    {
        var submodules = await submoduleRepository.GetByModuleIdAsync(moduleId, cancellationToken);

        return SubmoduleConverter.Convert(submodules);
    }
}
