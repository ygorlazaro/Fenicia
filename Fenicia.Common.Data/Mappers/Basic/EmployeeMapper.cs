using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Data.Requests.Basic;
using Fenicia.Common.Data.Responses.Basic;

namespace Fenicia.Common.Data.Mappers.Basic;

public static class EmployeeMapper
{
    public static List<EmployeeResponse> Map(List<EmployeeModel> models)
    {
        return [.. models.Select(Map)];
    }

    public static EmployeeResponse Map(EmployeeModel model)
    {
        return new EmployeeResponse
        {
            Id = model.Id,
            Name = model.Name,
            Cpf = model.Cpf,
            PositionId = model.PositionId,
            City = model.City,
            Complement = model.Complement,
            Neighborhood = model.Neighborhood,
            Number = model.Number,
            StateId = model.StateId,
            Street = model.Street,
            ZipCode = model.ZipCode
        };
    }

    public static EmployeeModel Map(EmployeeRequest request)
    {
        return new EmployeeModel
        {
            Id = request.Id,
            Name = request.Name,
            Cpf = request.Cpf,
            PositionId = request.PositionId,
            City = request.City,
            Complement = request.Complement,
            Neighborhood = request.Neighborhood,
            Number = request.Number,
            StateId = request.StateId,
            Street = request.Street,
            ZipCode = request.ZipCode
        };
    }
}
