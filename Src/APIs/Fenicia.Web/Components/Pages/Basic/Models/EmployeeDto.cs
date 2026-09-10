namespace Fenicia.Web.Components.Pages.Basic.Models;

public class EmployeeDto : IEquatable<EmployeeDto>, Fenicia.Web.Services.ICrudItem
{
    public Guid Id { get; init; }

    public string Name { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Document { get; set; }

    public Guid PositionId { get; set; }

    public string? PositionName { get; set; }

    public AddressDto? Address { get; set; }

    public bool Equals(EmployeeDto? other)
    {
        if (other is null)
        {
            return false;
        }

        return Id == other.Id && Name == other.Name;
    }

    public override bool Equals(object? obj) => Equals(obj as EmployeeDto);

    public override int GetHashCode() => HashCode.Combine(Id, Name);
}
