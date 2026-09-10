namespace Fenicia.Web.Components.Pages.Basic.Models;

public class AddressDto
{
    public string Street { get; set; } = string.Empty;

    public string Number { get; set; } = string.Empty;

    public string? Complement { get; set; }

    public string? Neighborhood { get; set; }

    public string ZipCode { get; set; } = string.Empty;

    public Guid StateId { get; set; }

    public string? StateName { get; set; }

    public string City { get; set; } = string.Empty;

    public string? Country { get; set; }
}
