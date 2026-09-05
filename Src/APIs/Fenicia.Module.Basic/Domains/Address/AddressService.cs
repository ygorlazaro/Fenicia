using Fenicia.Common.Data.Models.Auth;
using Fenicia.Module.Basic.Domains.Address.DTOs;
using Fenicia.Module.Basic.Domains.Address.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Address;

public sealed class AddressService(IAddressRepository addressRepository) : IAddressService
{
    public AddressService()
        : this(null!)
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

        var created = await addressRepository.Query()
            .Include(a => a.State)
            .FirstAsync(a => a.Id == address.Id, cancellationToken);

        return created.MapToAddressResponse();
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

        var reloaded = await addressRepository.Query()
            .Include(a => a.State)
            .FirstAsync(a => a.Id == id, cancellationToken);

        return reloaded.MapToAddressResponse();
    }

    public async Task<AddressResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var address = await addressRepository.Query()
            .Include(a => a.State)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

        return address?.MapToAddressResponse();
    }
}