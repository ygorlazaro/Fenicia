using Fenicia.Common.Data.Models.Basic;

namespace Fenicia.Common.Data.Responses.Basic;

public class PersonResponse
{
    public PersonResponse(PersonModel model)
    {
        this.Name = model.Name;
        this.Cpf = model.Cpf;
        this.City = model.City;
        this.Complement = model.Complement;
        this.Neighborhood = model.Neighborhood;
        this.Number = model.Number;
        this.StateId = model.StateId;
        this.Street = model.Street;
        this.ZipCode = model.ZipCode;
        this.PhoneNumber = model.PhoneNumber;
    }

    public string Name { get; set; }

    public string? Cpf { get; set; }

    public string City { get; set; }

    public string Complement { get; set; }

    public string Neighborhood { get; set; }

    public string Number { get; set; }

    public Guid StateId { get; set; }

    public string Street { get; set; }

    public string ZipCode { get; set; }

    public string PhoneNumber { get; set; }
}