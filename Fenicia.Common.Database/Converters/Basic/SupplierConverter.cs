using Fenicia.Common.Database.Models.Basic;
using Fenicia.Common.Database.Requests.Basic;
using Fenicia.Common.Database.Responses.Basic;

namespace Fenicia.Common.Database.Converters.Basic;

public static class SupplierConverter
{
    public static SupplierModel Convert(SupplierRequest request)
    {
        return new SupplierModel
        {
            Cpf = request.Cpf,
            Name = request.Name,
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

    public static SupplierResponse Convert(SupplierModel model)
    {
        return new SupplierResponse
        {
            Cpf = model.Cpf,
            Name = model.Name,
            Id = model.Id,
            City = model.City,
            Complement = model.Complement,
            Neighborhood = model.Neighborhood,
            Number = model.Number,
            StateId = model.StateId,
            Street = model.Street,
            ZipCode = model.ZipCode
        };
    }

    public static List<SupplierResponse> Convert(List<SupplierModel> suppliers)
    {
        return suppliers.Select(Convert).ToList();
    }
}
