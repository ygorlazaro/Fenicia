using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.DTOs.Basic.Customer;
using Riok.Mapperly.Abstractions;

namespace Fenicia.Module.Basic.Domains.Customer;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.None)]
public partial class CustomerMapper
{
    [MapProperty("Person.Name", nameof(GetAllCustomerResponse.Name))]
    [MapProperty("Person.Email", nameof(GetAllCustomerResponse.Email))]
    [MapProperty("Person.PhoneNumber", nameof(GetAllCustomerResponse.PhoneNumber))]
    [MapProperty("Person.Document", nameof(GetAllCustomerResponse.Document))]
    [MapProperty(nameof(CustomerModel.PersonId), nameof(GetAllCustomerResponse.PersonId))]
    public partial GetAllCustomerResponse MapToGetAllCustomerResponse(CustomerModel customer);

    [MapProperty("Person.Name", nameof(GetCustomerByIdResponse.Name))]
    [MapProperty("Person.Email", nameof(GetCustomerByIdResponse.Email))]
    [MapProperty("Person.PhoneNumber", nameof(GetCustomerByIdResponse.PhoneNumber))]
    [MapProperty("Person.Document", nameof(GetCustomerByIdResponse.Document))]
    [MapProperty(nameof(CustomerModel.PersonId), nameof(GetCustomerByIdResponse.PersonId))]
    public partial GetCustomerByIdResponse MapToGetCustomerByIdResponse(CustomerModel customer);
}
