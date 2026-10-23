using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.DTOs.Auth.Address;
using Fenicia.Common.DTOs.Basic.DataSource;
using Fenicia.Common.DTOs.Basic.Employee;

namespace Fenicia.Module.Basic.Domains.Employee;

/// <summary>
/// Mapper class for mapping employee data.
/// </summary>
public static class EmployeeMapper
{
    /// <summary>
    /// Maps an EmployeeModel to an EmployeeResponse.
    /// </summary>
    /// <param name="employee">The employee model.</param>
    /// <returns>The employee response.</returns>
    public static EmployeeResponse MapToResponse(EmployeeModel employee)
    {
        var address = employee.Person.PersonAddresses.FirstOrDefault()?.Address;
        return new EmployeeResponse(
            employee.Id,
            employee.PositionId,
            employee.PersonId,
            employee.Person.Name,
            employee.Person.Email,
            employee.Person.PhoneNumber,
            employee.Person.Document,
            employee.Position?.Name,
            address != null
                ? new AddressResponse(
                    address.Id,
                    address.Street,
                    address.Number,
                    address.Complement,
                    address.Neighborhood,
                    address.ZipCode!,
                    address.StateId,
                    address.State?.Name,
                    address.City,
                    address.Country)
                : null);
    }

    /// <summary>
    /// Maps an EmployeeModel to an EmployeeDataSourceResponse.
    /// </summary>
    /// <param name="employee">The employee model.</param>
    /// <returns>The employee data source response.</returns>
    public static EmployeeDataSourceResponse MapToDataSourceResponse(EmployeeModel employee)
    {
        return new EmployeeDataSourceResponse(employee.Id, employee.Person.Name);
    }

    /// <summary>
    /// Maps an EmployeeRequest to an EmployeeModel.
    /// </summary>
    /// <param name="request">The employee request.</param>
    /// <param name="companyId">The company ID.</param>
    /// <param name="personId">The person ID.</param>
    /// <returns>The employee model.</returns>
    public static EmployeeModel MapToModel(EmployeeRequest request, Guid companyId, Guid personId)
    {
        return new EmployeeModel
        {
            Id = request.Id,
            PositionId = request.PositionId,
            PersonId = personId,
            CompanyId = companyId
        };
    }
}