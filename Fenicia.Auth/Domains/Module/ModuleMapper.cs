using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.DTOs.Auth.Module;
using Riok.Mapperly.Abstractions;

namespace Fenicia.Auth.Domains.Module;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class ModuleMapper
{
    internal partial ModuleResponse MapToModuleResponse(ModuleModel module);

    internal partial ModuleByUserResponse MapToModuleByUserResponse(ModuleModel module);

    public partial ModuleResponse ModuleResponse(ModuleModel module);
}
