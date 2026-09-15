using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.DTOs.Auth.Module;
using Riok.Mapperly.Abstractions;

namespace Fenicia.Auth.Domains.Module;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class ModuleMapper
{
    internal partial GetModuleResponse MapToGetModuleResponse(ModuleModel module);

    internal partial GetUserModulesResponse MapToGetUserModulesResponse(ModuleModel module);
}
