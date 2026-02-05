using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Responses.Auth;

namespace Fenicia.Common.Data.Converters.Auth;

public static class ModuleConverter
{
    public static ModuleModel Convert(ModuleResponse module)
    {
        return new ModuleModel
        {
            Id = module.Id,
            Name = module.Name,
            Price = module.Price,
            Type = module.Type
        };
    }

    public static List<ModuleModel> Convert(List<ModuleResponse> modules)
    {
        return [.. modules.Select(Convert)];
    }
}
