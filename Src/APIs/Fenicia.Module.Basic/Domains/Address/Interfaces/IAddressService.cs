using Fenicia.Module.Basic.Domains.Address.DTOs;

namespace Fenicia.Module.Basic.Domains.Address.Interfaces;

public interface IAddressService
{
    Task<AddressResponse> AddAsync(AddressCommand command, CancellationToken cancellationToken = default);

    Task<AddressResponse?> UpdateAsync(Guid id, AddressCommand command, CancellationToken cancellationToken = default);

    Task<AddressResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}