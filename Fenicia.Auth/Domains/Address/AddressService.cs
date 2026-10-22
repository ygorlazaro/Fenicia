using Fenicia.Auth.Domains.Address.Interfaces;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.DTOs.Auth.Address;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Address;

/// <summary>
/// Service responsible for managing address-related operations.
/// </summary>
/// <param name="repository">The repository for managing address data.</param>
public sealed class AddressService(IAddressRepository repository) : IAddressService
{
    /// <summary>
    /// Adds a new address.
    /// </summary>
    /// <param name="command">The address request containing the address details.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The created address response.</returns>
    public async Task<AddressResponse> AddAsync(
        AddressRequest command,
        CancellationToken cancellationToken = default)
    {
        var address = new AddressModel
        {
            Id = Guid.NewGuid(),
            Street = command.Street,
            Number = command.Number,
            Complement = command.Complement,
            Neighborhood = command.Neighborhood,
            ZipCode = command.ZipCode,
            StateId = command.StateId,
            City = command.City,
            Country = command.Country
        };

        await repository.InsertAsync(address, cancellationToken);

        var saved = await ReloadAsync(address.Id, cancellationToken);

        return AddressMapper.MapToAddressResponse(saved);
    }

    /// <summary>
    /// Updates an existing address.
    /// </summary>
    /// <param name="id">The unique identifier of the address to update.</param>
    /// <param name="command">The address request containing the updated address details.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The updated address response, or null if not found.</returns>
    public async Task<AddressResponse?> UpdateAsync(
        Guid id,
        AddressRequest command,
        CancellationToken cancellationToken = default)
    {
        var address = new AddressModel
        {
            Id = id,
            Street = command.Street,
            Number = command.Number,
            Complement = command.Complement,
            Neighborhood = command.Neighborhood,
            ZipCode = command.ZipCode,
            StateId = command.StateId,
            City = command.City,
            Country = command.Country
        };

        var updated = await repository.UpdateAsync(id, address, cancellationToken);

        if (updated is null)
        {
            return null;
        }

        var reloaded = await ReloadAsync(id, cancellationToken);

        return AddressMapper.MapToAddressResponse(reloaded);
    }

    private async Task<AddressModel> ReloadAsync(Guid id, CancellationToken cancellationToken)
    {
        return await repository.GetAllQuery()
            .Include(a => a.State)
            .FirstAsync(a => a.Id == id, cancellationToken);
    }
}