using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.DTOs.Auth.Address;
using Fenicia.Module.Basic.Domains.Address.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Address;

public sealed class AddressService(IAddressRepository repository) : IAddressService
{
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

        return new AddressResponse(
            saved.Id,
            saved.Street,
            saved.Number,
            saved.Complement,
            saved.Neighborhood,
            saved.ZipCode!,
            saved.StateId,
            saved.State?.Name,
            saved.City,
            saved.Country);
    }

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

        return new AddressResponse(
            reloaded.Id,
            reloaded.Street,
            reloaded.Number,
            reloaded.Complement,
            reloaded.Neighborhood,
            reloaded.ZipCode!,
            reloaded.StateId,
            reloaded.State?.Name,
            reloaded.City,
            reloaded.Country);
    }

    private async Task<AddressModel> ReloadAsync(Guid id, CancellationToken cancellationToken)
    {
        return await repository.GetAllQuery()
            .Include(a => a.State)
            .FirstAsync(a => a.Id == id, cancellationToken);
    }
}
