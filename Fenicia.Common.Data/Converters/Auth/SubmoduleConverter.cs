using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Responses.Auth;

namespace Fenicia.Common.Data.Converters.Auth;

public static class SubmoduleConverter
{
    public static SubmoduleResponse Convert(SubmoduleModel submodule)
    {
        return new SubmoduleResponse
        {
            Id = submodule.Id,
            Name = submodule.Name,
            Description = submodule.Description
        };
    }

    public static List<SubmoduleResponse> Convert(List<SubmoduleModel> submodules)
    {
        return submodules.Select(Convert).ToList();
    }
}
