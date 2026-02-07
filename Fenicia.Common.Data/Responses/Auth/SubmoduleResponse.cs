using Fenicia.Common.Data.Models.Auth;

namespace Fenicia.Common.Data.Responses.Auth;

[Serializable]
public class SubmoduleResponse
{
    public Guid Id
    {
        get; set;
    }

    public string Name { get; set; } = null!;

    public string? Description
    {
        get; set;
    }

    public string Route { get; set; } = null!;

    public static SubmoduleResponse Map(SubmoduleModel submodules)
    {
        return new SubmoduleResponse
        {
            Id = submodules.Id,
            Name = submodules.Name,
            Description = submodules.Description,
            Route = submodules.Route
        };
    }

    public static SubmoduleResponse[] Map(List<SubmoduleModel> submodules)
    {
        return [.. submodules.Select(Map)];
    }
}