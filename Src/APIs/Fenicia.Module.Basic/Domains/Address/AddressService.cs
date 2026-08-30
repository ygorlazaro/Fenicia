using Fenicia.Common.Data.Models.Auth;
using Fenicia.Module.Basic.Domains.Address;
using Fenicia.Module.Basic.Domains.Address.DTOs;

namespace Fenicia.Module.Basic.Domains.Address;

public class AddressService(IAddressRepository addressRepository)
{
    public async Task<AddressResponse> AddAsync(AddressCommand command, CancellationToken ct)
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

        var created = await addressRepository.InsertAsync(address, ct);

        return created.MapToAddressResponse();
    }

    public async Task<AddressResponse?> UpdateAsync(Guid id, AddressCommand command, CancellationToken ct)
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

        var updated = await addressRepository.UpdateAsync(id, address, ct);

        return updated?.MapToAddressResponse();
    }

    public async Task<AddressResponse?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var address = await addressRepository.GetByIdAsync(id, ct);

        return address?.MapToAddressResponse();
    }
}
