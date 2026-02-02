using Fenicia.Common.Database.Models.Auth;

namespace Fenicia.Common.Database.Responses;

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

    public static SubmoduleResponse Convert(SubmoduleModel submodules)
    {
        return new SubmoduleResponse
        {
            Id = submodules.Id,
            Name = submodules.Name,
            Description = submodules.Description,
            Route = submodules.Route
        };
    }

    public static SubmoduleResponse[] Convert(List<SubmoduleModel> submodules)
    {
        return [.. submodules.Select(SubmoduleResponse.Convert)];
    }
}
