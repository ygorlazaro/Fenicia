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
            PositionId = model.PositionId,
            Person = PersonMapper.Map(model.Person)
        };
    }

    public static EmployeeModel Map(EmployeeRequest request)
    {
        return new EmployeeModel
        {
            Id = request.Id,
            PositionId = request.PositionId,
            Person = PersonMapper.Map(request.Person)
        };
    }
}