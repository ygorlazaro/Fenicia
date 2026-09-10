namespace Fenicia.Web.Components.Pages.Basic.Models;

using Fenicia.Web.Services;

public class PositionDto : IEquatable<PositionDto>, ICrudItem
{
    public Guid Id { get; init; }

    public string Name { get; set; } = string.Empty;

    public bool Equals(PositionDto? other)
    {
        if (other is null)
        {
            return false;
        }

        return Id == other.Id && Name == other.Name;
    }

    public override bool Equals(object? obj) => Equals(obj as PositionDto);

    public override int GetHashCode() => HashCode.Combine(Id, Name);
}
