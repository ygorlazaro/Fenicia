using Fenicia.Common.DTOs.Auth.PersonAddress;

namespace Fenicia.Module.Basic.Domains.PersonAddress.Interfaces;

public interface IPersonAddressService
{
    Task<PersonAddressResponse> InsertAsync(
        PersonAddressRequest command,
        Guid companyId,
        CancellationToken cancellationToken = default);
}
