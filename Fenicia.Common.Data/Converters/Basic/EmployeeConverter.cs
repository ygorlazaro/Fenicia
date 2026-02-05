using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Data.Requests.Basic;
using Fenicia.Common.Data.Responses.Basic;

namespace Fenicia.Common.Data.Converters.Basic;

public static class EmployeeConverter
{
    public static List<EmployeeResponse> Convert(List<EmployeeModel> models)
    {
        return models.Select(Convert).ToList();
    }

    public static EmployeeResponse Convert(EmployeeModel model)
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

    public static EmployeeModel Convert(EmployeeRequest request)
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
