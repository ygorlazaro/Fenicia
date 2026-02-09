namespace Fenicia.Common.Data.Requests.Basic;

public  class PersonRequest
{
    public string Name { get; set; } = null!;

    public string? Cpf { get; set; }

    public string City { get; set; } = null!;

    public string Complement { get; set; } = null!;

    public string Neighborhood { get; set; } = null!;

    public string Number { get; set; } = null!;

    public Guid StateId { get; set; }

    public string Street { get; set; } = null!;

    public string ZipCode { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;
}