using Fenicia.Web.Services;

namespace Fenicia.Web.Components.Pages.Basic.Models;

public class CustomerDto : IEquatable<CustomerDto>, ICrudItem
{
    public Guid Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string? Email { get; init; }

    public string? PhoneNumber { get; init; }

    public string? Document { get; init; }

    public AddressDto? Address { get; init; }

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
