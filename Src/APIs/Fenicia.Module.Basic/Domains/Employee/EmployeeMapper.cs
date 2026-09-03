using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Address.DTOs;
using Fenicia.Module.Basic.Domains.Employee.DTOs;
using Riok.Mapperly.Abstractions;

namespace Fenicia.Module.Basic.Domains.Employee;

[Mapper]
public static partial class EmployeeMapper
{
    public static GetAllEmployeeResponse MapToGetAllEmployeeResponse(this EmployeeModel employee)
    {
        var personAddress = employee.Person.PersonAddresses.FirstOrDefault();
        var address = personAddress?.Address;

        var addressResponse = address != null
            ? new AddressResponse(
                address.Id,
                address.Street,
                address.Number,
                address.Complement,
                address.Neighborhood,
                address.ZipCode!,
                address.StateId,
                address.State.Name,
                address.City,
                address.Country)
            : null;
        return new GetAllEmployeeResponse(
            employee.Id,
            employee.PositionId,
            employee.PersonId,
            employee.Person.Name,
            employee.Person.Email,
            employee.Person.PhoneNumber,
            employee.Person.Document,
            employee.Position.Name,
            addressResponse);
    }

    public static GetEmployeeByIdResponse MapToGetEmployeeByIdResponse(this EmployeeModel employee)
    {
        var personAddress = employee.Person.PersonAddresses.FirstOrDefault();
        var address = personAddress?.Address;

        var addressResponse = address != null
            ? new AddressResponse(
                address.Id,
                address.Street,
                address.Number,
                address.Complement,
                address.Neighborhood,
                address.ZipCode!,
                address.StateId,
                address.State.Name,
                address.City,
                address.Country)
            : null;
        return new GetEmployeeByIdResponse(
            employee.Id,
            employee.PositionId,
            employee.PersonId,
            employee.Person.Name,
            employee.Person.Email,
            employee.Person.PhoneNumber,
            employee.Person.Document,
            addressResponse);
    }

    public static GetEmployeesByPositionIdResponse MapToGetEmployeesByPositionIdResponse(this EmployeeModel employee)
    {
        var personAddress = employee.Person.PersonAddresses.FirstOrDefault();
        var address = personAddress?.Address;

        var addressResponse = address != null
            ? new AddressResponse(
                address.Id,
                address.Street,
                address.Number,
                address.Complement,
                address.Neighborhood,
                address.ZipCode!,
                address.StateId,
                address.State.Name,
                address.City,
                address.Country)
            : null;
        return new GetEmployeesByPositionIdResponse(
            employee.Id,
            employee.PositionId,
            employee.PersonId,
            employee.Person.Name,
            employee.Person.Email,
            employee.Person.PhoneNumber,
            employee.Person.Document,
            employee.Position.Name,
            addressResponse);
    }
}