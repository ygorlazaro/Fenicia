using Fenicia.Common.Data.Mappers.Auth;
using Fenicia.Common.Data.Responses.Auth;

namespace Fenicia.Auth.Domains.Submodule;

public class SubmoduleService(ISubmoduleRepository submoduleRepository) : ISubmoduleService
{
    public async Task<List<SubmoduleResponse>> GetByModuleIdAsync(Guid moduleId, CancellationToken ct)
    {
        var submodules = await submoduleRepository.GetByModuleIdAsync(moduleId, ct);

        return SubmoduleMapper.Map(submodules);
    }
}