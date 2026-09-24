using Fenicia.Common.DTOs.Auth.Address;

namespace Fenicia.Auth.Domains.Address.Interfaces;

/// <summary>
/// Defines the contract for a service that manages address-related operations.
/// </summary>
public interface IAddressService
{
    /// <summary>
    /// Adds a new address.
    /// </summary>
    /// <param name="command">The address request containing the address details.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The created address response.</returns>
    Task<AddressResponse> AddAsync(AddressRequest command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing address.
    /// </summary>
    /// <param name="id">The unique identifier of the address to update.</param>
    /// <param name="command">The address request containing the updated address details.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The updated address response, or null if not found.</returns>
    Task<AddressResponse?> UpdateAsync(Guid id, AddressRequest command, CancellationToken cancellationToken = default);
}