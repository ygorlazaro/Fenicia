using Fenicia.Common.DTOs.Auth.Address;

namespace Fenicia.Module.Basic.Domains.Address.Interfaces;

public interface IAddressService
{
    Task<AddressResponse> AddAsync(AddressRequest command, CancellationToken cancellationToken = default);

    Task<AddressResponse?> UpdateAsync(Guid id, AddressRequest command, CancellationToken cancellationToken = default);
}
