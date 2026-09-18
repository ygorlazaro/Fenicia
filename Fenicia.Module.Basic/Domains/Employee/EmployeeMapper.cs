using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.DTOs.Basic.Employee;
using Riok.Mapperly.Abstractions;

namespace Fenicia.Module.Basic.Domains.Employee;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.None)]
public partial class EmployeeMapper
{
    [MapProperty("Person.Name", nameof(GetAllEmployeeResponse.Name))]
    [MapProperty("Person.Email", nameof(GetAllEmployeeResponse.Email))]
    [MapProperty("Person.PhoneNumber", nameof(GetAllEmployeeResponse.PhoneNumber))]
    [MapProperty("Person.Document", nameof(GetAllEmployeeResponse.Document))]
    [MapProperty("Position.Name", nameof(GetAllEmployeeResponse.PositionName))]
    public partial GetAllEmployeeResponse MapToGetAllEmployeeResponse(EmployeeModel employee);

    [MapProperty("Person.Name", nameof(GetEmployeeByIdResponse.Name))]
    [MapProperty("Person.Email", nameof(GetEmployeeByIdResponse.Email))]
    [MapProperty("Person.PhoneNumber", nameof(GetEmployeeByIdResponse.PhoneNumber))]
    [MapProperty("Person.Document", nameof(GetEmployeeByIdResponse.Document))]
    public partial GetEmployeeByIdResponse MapToGetEmployeeByIdResponse(EmployeeModel employee);

    [MapProperty("Person.Name", nameof(GetEmployeesByPositionIdResponse.Name))]
    [MapProperty("Person.Email", nameof(GetEmployeesByPositionIdResponse.Email))]
    [MapProperty("Person.PhoneNumber", nameof(GetEmployeesByPositionIdResponse.PhoneNumber))]
    [MapProperty("Person.Document", nameof(GetEmployeesByPositionIdResponse.Document))]
    [MapProperty("Position.Name", nameof(GetEmployeesByPositionIdResponse.PositionName))]
    public partial GetEmployeesByPositionIdResponse MapToGetEmployeesByPositionIdResponse(EmployeeModel employee);
}
