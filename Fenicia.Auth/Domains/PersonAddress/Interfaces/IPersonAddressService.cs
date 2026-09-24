using Fenicia.Common.DTOs.Auth.PersonAddress;

namespace Fenicia.Auth.Domains.PersonAddress.Interfaces;

/// <summary>
/// Defines the contract for a service that manages person address relationship operations.
/// </summary>
public interface IPersonAddressService
{
    /// <summary>
    /// Inserts a new person address relationship.
    /// </summary>
    /// <param name="command">The person address request containing the person and address identifiers.</param>
    /// <param name="companyId">The unique identifier of the company.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The created person address response.</returns>
    Task<PersonAddressResponse> InsertAsync(
        PersonAddressRequest command,
        Guid companyId,
        CancellationToken cancellationToken = default);
}