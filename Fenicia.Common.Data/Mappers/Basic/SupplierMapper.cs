using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Data.Requests.Basic;
using Fenicia.Common.Data.Responses.Basic;

namespace Fenicia.Common.Data.Mappers.Basic;

public static class SupplierMapper
{
    public static SupplierModel Map(SupplierRequest request)
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

    public static SupplierResponse Map(SupplierModel model)
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

    public static List<SupplierResponse> Map(List<SupplierModel> suppliers)
    {
        return [.. suppliers.Select(Map)];
    }
}
