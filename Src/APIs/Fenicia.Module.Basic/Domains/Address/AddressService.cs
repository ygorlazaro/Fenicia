using Fenicia.Common.Data.Models.Auth;
using Fenicia.Module.Basic.Domains.Address;
using Fenicia.Module.Basic.Domains.Address.DTOs;

namespace Fenicia.Module.Basic.Domains.Address;

public class AddressService
{
    private readonly IAddressRepository _addressRepository;

    public AddressService()
        : this(null!)
    {
    }

    public AddressService(IAddressRepository addressRepository)
    {
        _addressRepository = addressRepository;
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

        var created = await _addressRepository.InsertAsync(address, cancellationToken);

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

        var updated = await _addressRepository.UpdateAsync(id, address, cancellationToken);

        return updated?.MapToAddressResponse();
    }

    public virtual async Task<AddressResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var address = await _addressRepository.GetByIdAsync(id, cancellationToken);

        return address?.MapToAddressResponse();
    }
}
