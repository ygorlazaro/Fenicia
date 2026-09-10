namespace Fenicia.Web.Components.Pages.Basic.Models;

public class SupplierDto : IEquatable<SupplierDto>, global::Fenicia.Web.Services.ICrudItem
{
    public Guid Id { get; init; }

    public string Name { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Document { get; set; }

    public AddressDto? Address { get; set; }

    public bool Equals(SupplierDto? other)
    {
        if (other is null)
        {
            return false;
        }

        return Id == other.Id && Name == other.Name;
    }

    public override bool Equals(object? obj) => Equals(obj as SupplierDto);

    public override int GetHashCode() => HashCode.Combine(Id, Name);
}
