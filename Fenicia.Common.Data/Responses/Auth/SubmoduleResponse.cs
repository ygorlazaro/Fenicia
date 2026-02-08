using Fenicia.Common.Data.Models.Auth;

namespace Fenicia.Common.Data.Responses.Auth;

[Serializable]
public class SubmoduleResponse()
{
    public SubmoduleResponse(SubmoduleModel model) : this()
    {
        this.Id = model.Id;
        this.Name = model.Name;
        this.Description = model.Description;
        this.Route = model.Route;
    }

    public Guid Id { get; set; } = Guid.Empty;

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string Route { get; set; } = string.Empty;
}