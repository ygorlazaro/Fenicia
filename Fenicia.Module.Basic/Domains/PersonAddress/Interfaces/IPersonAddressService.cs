using Fenicia.Common.DTOs.Basic.PersonAddress;

namespace Fenicia.Module.Basic.Domains.PersonAddress.Interfaces;

public interface IPersonAddressService
{
    Task<GetPersonAddressResponse> InsertAsync(
        AddPersonAddressCommand command,
        Guid companyId,
        CancellationToken cancellationToken = default);
}
