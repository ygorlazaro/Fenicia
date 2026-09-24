using Fenicia.Auth.Domains.PersonAddress.Interfaces;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.DTOs.Auth.PersonAddress;

namespace Fenicia.Auth.Domains.PersonAddress;

/// <summary>
/// Service responsible for managing person address relationship operations.
/// </summary>
/// <param name="repository">The repository for managing person address relationship data.</param>
public sealed class PersonAddressService(IPersonAddressRepository repository) : IPersonAddressService
{
    /// <summary>
    /// Inserts a new person address relationship.
    /// </summary>
    /// <param name="command">The person address request containing the person and address identifiers.</param>
    /// <param name="companyId">The unique identifier of the company.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The created person address response.</returns>
    public async Task<PersonAddressResponse> InsertAsync(
        PersonAddressRequest command,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        var personAddress = new PersonAddressModel
        {
            Id = Guid.NewGuid(),
            PersonId = command.PersonId,
            AddressId = command.AddressId,
            CompanyId = companyId
        };

        var result = await repository.InsertAsync(personAddress, cancellationToken);
        return new PersonAddressResponse(result.Id, result.PersonId, result.AddressId, result.Person.Name);
    }
}