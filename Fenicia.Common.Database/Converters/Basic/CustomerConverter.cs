using Fenicia.Common.Database.Models.Basic;
using Fenicia.Common.Database.Requests.Basic;
using Fenicia.Common.Database.Responses.Basic;

namespace Fenicia.Common.Database.Converters.Basic;

public static class CustomerConverter
{
    public static CustomerResponse Convert(CustomerModel model)
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

    public static CustomerModel Convert(CustomerRequest request)
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

    public static List<CustomerResponse> Convert(List<CustomerModel> model)
    {
        return model.Select(Convert).ToList();
    }
}
