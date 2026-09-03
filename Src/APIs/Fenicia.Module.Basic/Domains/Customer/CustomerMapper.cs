using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Address.DTOs;
using Fenicia.Module.Basic.Domains.Customer.DTOs;
using Riok.Mapperly.Abstractions;

namespace Fenicia.Module.Basic.Domains.Customer;

[Mapper]
public static partial class CustomerMapper
{
    public static GetAllCustomerResponse MapToGetAllCustomerResponse(this CustomerModel customer)
    {
        var personAddress = customer.Person.PersonAddresses.FirstOrDefault();
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
        return new GetAllCustomerResponse(
            customer.Id,
            customer.PersonId,
            customer.Person.Name,
            customer.Person.Email,
            customer.Person.PhoneNumber,
            customer.Person.Document,
            addressResponse);
    }

    public static GetCustomerByIdResponse MapToGetCustomerByIdResponse(this CustomerModel customer)
    {
        var personAddress = customer.Person.PersonAddresses.FirstOrDefault();
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
        return new GetCustomerByIdResponse(
            customer.Id,
            customer.PersonId,
            customer.Person.Name,
            customer.Person.Email,
            customer.Person.PhoneNumber,
            customer.Person.Document,
            addressResponse);
    }
}