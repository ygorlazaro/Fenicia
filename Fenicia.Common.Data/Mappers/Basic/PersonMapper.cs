using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Data.Requests.Basic;
using Fenicia.Common.Data.Responses;

namespace Fenicia.Common.Data.Mappers.Basic;

public class PersonMapper
{
    public static PersonResponse Map(PersonModel model)
    {
        return new PersonResponse
        {
            Name = model.Name,
            Cpf = model.Cpf,
            City = model.City,
            Complement = model.Complement,
            Neighborhood = model.Neighborhood,
            Number = model.Number,
            StateId = model.StateId,
            Street = model.Street,
            ZipCode = model.ZipCode,
            PhoneNumber = model.PhoneNumber
        };
    }

    public static PersonModel Map(PersonRequest request)
    {
        return new PersonModel
        {
            Name = request.Name,
            Cpf = request.Cpf,
            City = request.City,
            Complement = request.Complement,
            Neighborhood = request.Neighborhood,
            Number = request.Number,
            StateId = request.StateId,
            Street = request.Street,
            ZipCode = request.ZipCode,
            PhoneNumber = request.PhoneNumber
        };
    }
}