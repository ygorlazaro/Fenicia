using Fenicia.Common.Data.Models.Basic;

namespace Fenicia.Module.Basic.Domains.PersonAddress.Interfaces;

public interface IPersonAddressService
{
    Task<PersonAddressModel> InsertAsync(
        PersonAddressModel personAddress,
        Guid companyId,
        CancellationToken cancellationToken = default);
}