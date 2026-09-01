using Fenicia.Common.Data.Models.Auth;
using Fenicia.Module.Basic.Domains.Address.DTOs;

namespace Fenicia.Module.Basic.Domains.Address;

public class AddressService(IAddressRepository addressRepository)
{
    public AddressService()
        : this(null!)
    {
    }

    public virtual async Task<AddressResponse> AddAsync(AddressCommand command, CancellationToken cancellationToken = default)
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

        var created = await addressRepository.InsertAsync(address, cancellationToken);

        return created.MapToAddressResponse();
    }

    public virtual async Task<AddressResponse?> UpdateAsync(Guid id, AddressCommand command, CancellationToken cancellationToken = default)
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

        return updated?.MapToAddressResponse();
    }

    public virtual async Task<AddressResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var address = await addressRepository.GetByIdAsync(id, cancellationToken);

        return address?.MapToAddressResponse();
    }
}
