using Fenicia.Common;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.DTOs.Auth.Address;
using Fenicia.Common.DTOs.Auth.Person;
using Fenicia.Common.DTOs.Auth.PersonAddress;
using Fenicia.Common.DTOs.Basic.Customer;
using Fenicia.Common.DTOs.Basic.DataSource;
using Fenicia.Common.Exceptions;
using Fenicia.Auth.Domains.Address.Interfaces;
using Fenicia.Module.Basic.Domains.Customer.Interfaces;
using Fenicia.Module.Basic.Domains.Order.Interfaces;
using Fenicia.Auth.Domains.Person.Interfaces;
using Fenicia.Auth.Domains.PersonAddress.Interfaces;

namespace Fenicia.Module.Basic.Domains.Customer;

/// <summary>
/// Service implementation for managing customers in the basic module.
/// </summary>
/// <param name="repository">The customer repository.</param>
/// <param name="personService">The person service.</param>
/// <param name="addressService">The address service.</param>
/// <param name="personAddressService">The person address service.</param>
public sealed class CustomerService(
    ICustomerRepository repository,
    IPersonService personService,
    IAddressService addressService,
    IPersonAddressService personAddressService) : ICustomerService
{
    /// <inheritdoc />
    public async Task<Pagination<List<CustomerResponse>>> GetAllAsync(
        PaginationQuery query,
        CancellationToken cancellationToken = default)
    {
        var customers = await repository.GetAllWithDetailsAsync(query, cancellationToken);
        var total = await repository.CountAsync(cancellationToken);

        return CustomerMapper.MapToPaginatedCustomerResponse(customers, total, query);
    }

    /// <inheritdoc />
    public async Task<List<CustomerDataSourceResponse>> GetAllForDataSourceAsync(
        CancellationToken cancellationToken = default)
    {
        var customers = await repository.GetAllWithDetailsAsync(new PaginationQuery(1, int.MaxValue), cancellationToken);

        return [.. customers.Select(c => new CustomerDataSourceResponse(c.Id, c.Person.Name))];
    }

    /// <inheritdoc />
    public async Task<CustomerResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var customer = await repository.GetByIdWithDetailsAsync(id, cancellationToken);

        if (customer is null)
        {
            return null;
        }

        return CustomerMapper.MapToCustomerResponse(customer);
    }

    /// <inheritdoc />
    public async Task<CustomerResponse> AddAsync(
        CustomerRequest request,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        var personCommand = new PersonRequest(
            request.Name,
            request.Document,
            request.Email,
            request.PhoneNumber,
            null,
            null,
            null);

        Guid? addressId = null;

        if (request.Address != null)
        {
            var addressCommand = new AddressRequest(
                request.Address.Street,
                request.Address.Number,
                request.Address.Complement,
                request.Address.Neighborhood,
                request.Address.ZipCode,
                request.Address.StateId,
                request.Address.City,
                request.Address.Country);
            var createdAddress = await addressService.AddAsync(addressCommand, cancellationToken);
            addressId = createdAddress.Id;
        }

        var personResponse = await personService.InsertAsync(personCommand, companyId, cancellationToken);

        var customer = new CustomerModel
        {
            PersonId = personResponse.Id
        };

        await repository.InsertAsync(customer, cancellationToken);

        if (!addressId.HasValue)
        {
            return CustomerMapper.MapToCustomerResponse(customer);
        }

        var personAddressCommand = new PersonAddressRequest(personResponse.Id, addressId.Value);
        await personAddressService.InsertAsync(personAddressCommand, companyId, cancellationToken);

        return CustomerMapper.MapToCustomerResponse(customer);
    }

    /// <inheritdoc />
    public async Task<CustomerResponse?> UpdateAsync(
        Guid id,
        CustomerRequest request,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        var customer = await repository.GetByIdWithDetailsAsync(id, cancellationToken);

        if (customer is null)
        {
            return null;
        }

        customer.Person.Name = request.Name;
        customer.Person.Email = request.Email;
        customer.Person.Document = request.Document;
        customer.Person.PhoneNumber = request.PhoneNumber;

        if (request.Address != null)
        {
            var existingPersonAddress = customer.Person.PersonAddresses.FirstOrDefault();

            if (existingPersonAddress?.Address != null)
            {
                var addressCommand = new AddressRequest(
                    request.Address.Street,
                    request.Address.Number,
                    request.Address.Complement,
                    request.Address.Neighborhood,
                    request.Address.ZipCode,
                    request.Address.StateId,
                    request.Address.City,
                    request.Address.Country);
                await addressService.UpdateAsync(existingPersonAddress.Address.Id, addressCommand, cancellationToken);
            }
            else
            {
                var addressCommand = new AddressRequest(
                    request.Address.Street,
                    request.Address.Number,
                    request.Address.Complement,
                    request.Address.Neighborhood,
                    request.Address.ZipCode,
                    request.Address.StateId,
                    request.Address.City,
                    request.Address.Country);
                var createdAddress = await addressService.AddAsync(addressCommand, cancellationToken);

                var newPersonAddress = new Fenicia.Common.Data.Models.Auth.PersonAddressModel
                {
                    Id = Guid.NewGuid(),
                    PersonId = customer.PersonId,
                    AddressId = createdAddress.Id
                };
                var personAddressCommand = new PersonAddressRequest(newPersonAddress.PersonId, newPersonAddress.AddressId);
                await personAddressService.InsertAsync(personAddressCommand, companyId, cancellationToken);
            }
        }

        var personCommand = new PersonRequest(
            customer.Person.Name,
            customer.Person.Document,
            customer.Person.Email,
            customer.Person.PhoneNumber,
            null,
            null,
            null);

        await personService.UpdateAsync(customer.Person.Id, personCommand, companyId, cancellationToken);
        var updated = await repository.UpdateAsync(id, customer, cancellationToken) ??
                      throw new ForbiddenException();
        return CustomerMapper.MapToCustomerResponse(updated);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(
        Guid id,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        await repository.DeleteAsync(id, cancellationToken);
    }

    /// <inheritdoc />
    public Task<int> GetCountAsync(CancellationToken cancellationToken = default)
    {
        return repository.CountAsync(cancellationToken);
    }
}
