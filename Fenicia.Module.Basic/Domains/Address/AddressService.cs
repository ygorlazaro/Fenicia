using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.DTOs.Basic.Address;
using Fenicia.Module.Basic.Domains.Address.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Address;

public sealed class AddressService(IAddressRepository addressRepository, AddressMapper addressMapper) : IAddressService
{
    public AddressService()
        : this(null!, null!)
    {
    }

    public async Task<AddressResponse> AddAsync(
        AddressCommand command,
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

        await addressRepository.InsertAsync(address, cancellationToken);

        var saved = await ReloadAsync(address.Id, cancellationToken);

        return addressMapper.MapToAddressResponse(saved);
    }

    public async Task<AddressResponse?> UpdateAsync(
        Guid id,
        AddressCommand command,
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

        var updated = await addressRepository.UpdateAsync(id, address, cancellationToken);

        if (updated is null)
        {
            return null;
        }

        var reloaded = await ReloadAsync(id, cancellationToken);

        return addressMapper.MapToAddressResponse(reloaded);
    }

    private async Task<AddressModel> ReloadAsync(Guid id, CancellationToken cancellationToken)
    {
        return await addressRepository.GetAllQuery()
            .Include(a => a.State)
            .FirstAsync(a => a.Id == id, cancellationToken);
    }
}