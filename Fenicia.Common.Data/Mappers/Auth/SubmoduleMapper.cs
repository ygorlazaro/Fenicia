using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Responses.Auth;

namespace Fenicia.Common.Data.Mappers.Auth;

public static class SubmoduleMapper
{
    public static SubmoduleResponse Map(SubmoduleModel submodule)
    {
        return new SubmoduleResponse
        {
            Id = submodule.Id,
            Name = submodule.Name,
            Description = submodule.Description
        };
    }

    public static List<SubmoduleResponse> Map(List<SubmoduleModel> submodules)
    {
        return [.. submodules.Select(Map)];
    }
}