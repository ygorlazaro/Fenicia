using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Data.Requests.Basic;
using Fenicia.Common.Data.Responses.Basic;

namespace Fenicia.Common.Data.Mappers.Basic;

public static class CustomerMapper
{
    public static CustomerResponse Map(CustomerModel model)
    {
        return new CustomerResponse
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
            Id = model.Id
        };
    }

    public static CustomerModel Map(CustomerRequest request)
    {
        return new CustomerModel
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
            Id = request.Id
        };
    }

    public static List<CustomerResponse> Map(List<CustomerModel> model)
    {
        return [.. model.Select(Map)];
    }
}
