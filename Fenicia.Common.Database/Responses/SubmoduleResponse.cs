namespace Fenicia.Common.Database.Responses;

using System.Collections.Generic;

using Fenicia.Common.Database.Models.Auth;

[Serializable]
public class SubmoduleResponse
{
    public Guid Id
    {
        get; set;
    }

    public string Name { get; set; } = default!;

    public string? Description
    {
        get; set;
    }

    public string Route { get; set; } = default!;

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
