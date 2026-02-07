using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Responses.Auth;

namespace Fenicia.Common.Data.Mappers.Auth;

public static class ModuleMapper
{
    public static ModuleModel Map(ModuleResponse module)
    {
        return new ModuleModel
        {
            Id = module.Id,
            Name = module.Name,
            Price = module.Price,
            Type = module.Type
        };
    }

    public static List<ModuleModel> Map(List<ModuleResponse> modules)
    {
        return [.. modules.Select(Map)];
    }

    public static ModuleResponse Map(ModuleModel module)
    {
        return new ModuleResponse
        {
            Id = module.Id,
            Name = module.Name,
            Price = module.Price,
            Type = module.Type,
            Submodules = SubmoduleResponse.Map(module.Submodules)
        };
    }

    public static List<ModuleResponse> Map(List<ModuleModel> modules)
    {
        return [.. modules.Select(Map)];
    }
}
