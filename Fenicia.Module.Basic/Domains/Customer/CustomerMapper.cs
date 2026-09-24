using Fenicia.Common;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.DTOs.Auth.Address;
using Fenicia.Common.DTOs.Basic.Customer;

namespace Fenicia.Module.Basic.Domains.Customer;

/// <summary>
/// Mapper class for mapping between CustomerModel and CustomerResponse/CustomerRequest.
/// </summary>
public static class CustomerMapper
{
    /// <summary>
    /// Maps a CustomerModel to a CustomerResponse.
    /// </summary>
    /// <param name="customer">The customer model.</param>
    /// <returns>The customer response.</returns>
    public static CustomerResponse MapToCustomerResponse(CustomerModel customer)
    {
        var address = customer.Person?.PersonAddresses?.FirstOrDefault()?.Address;
        return new CustomerResponse(
            customer.Id,
            customer.PersonId,
            customer.Person?.Name ?? string.Empty,
            customer.Person?.Email,
            customer.Person?.PhoneNumber,
            customer.Person?.Document,
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
    /// Maps a CustomerRequest to a CustomerModel.
    /// </summary>
    /// <param name="request">The customer request.</param>
    /// <returns>The customer model.</returns>
    public static CustomerModel MapToCustomerModel(CustomerRequest request)
    {
        return new CustomerModel
        {
            Id = request.Id,
            PersonId = Guid.NewGuid()
        };
    }

    /// <summary>
    /// Maps a list of CustomerModels to a paginated list of CustomerResponses.
    /// </summary>
    /// <param name="customers">The list of customer models.</param>
    /// <param name="total">The total count.</param>
    /// <param name="query">The pagination query.</param>
    /// <returns>The paginated customer responses.</returns>
    public static Pagination<List<CustomerResponse>> MapToPaginatedCustomerResponse(
        IEnumerable<CustomerModel> customers,
        int total,
        PaginationQuery query)
    {
        var response = customers.Select(c =>
        {
            var address = c.Person?.PersonAddresses?.FirstOrDefault()?.Address;
            return new CustomerResponse(
                c.Id,
                c.PersonId,
                c.Person?.Name ?? string.Empty,
                c.Person?.Email,
                c.Person?.PhoneNumber,
                c.Person?.Document,
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
        }).ToList();

        return new Pagination<List<CustomerResponse>>(response, total, query);
    }
}