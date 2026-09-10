using Fenicia.Web.Services;

namespace Fenicia.Web.Components.Pages.Basic.Models;

public class CustomerDto : IEquatable<CustomerDto>, ICrudItem
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Document { get; set; }

    public AddressDto? Address { get; set; }

    public bool Equals(CustomerDto? other)
    {
        if (other is null)
        {
            return false;
        }

        return Id == other.Id && Name == other.Name;
    }

    public override bool Equals(object? obj) => Equals(obj as CustomerDto);

    public override int GetHashCode() => HashCode.Combine(Id, Name);
}
