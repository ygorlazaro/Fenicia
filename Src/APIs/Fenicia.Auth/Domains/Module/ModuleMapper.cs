using Fenicia.Auth.Domains.Module.DTOs;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Enums.Auth;

namespace Fenicia.Auth.Domains.Module;

public static partial class ModuleMapper
{
    public static GetModuleResponse MapToGetModuleResponse(this ModuleModel module)
    {
        return new GetModuleResponse(
            module.Id,
            module.Name,
            module.Type,
            module.Description,
            module.Icon,
            module.IsActive,
            module.SortOrder,
            module.Price);
    }

    public static GetUserModulesResponse MapToGetUserModulesResponse(this ModuleModel module)
    {
        return new GetUserModulesResponse(
            module.Id,
            module.Name,
            module.Type);
    }
}
