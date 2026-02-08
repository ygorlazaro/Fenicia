using Fenicia.Common.Data.Models.Basic;

namespace Fenicia.Common.Data.Responses.Basic;

public class AddressResponse
{
    public Guid Id { get; set; }

    public string Street { get; set; } = string.Empty;

    public string Number { get; set; } = string.Empty;

    public string? Complement { get; set; } = string.Empty;

    public string Neighborhood { get; set; } = string.Empty;

    public string ZipCode { get; set; } = string.Empty;

    public StateModel StateId { get; set; } = null!;

    public string City { get; set; } = string.Empty;
}