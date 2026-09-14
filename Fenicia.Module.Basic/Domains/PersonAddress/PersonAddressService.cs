using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.PersonAddress.Interfaces;

namespace Fenicia.Module.Basic.Domains.PersonAddress;

public sealed class PersonAddressService(IPersonAddressRepository personAddressRepository) : IPersonAddressService
{
    public PersonAddressService()
        : this(null!)
    {
    }

    public Task<PersonAddressModel> InsertAsync(
        PersonAddressModel personAddress,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        personAddress.CompanyId = companyId;
        return personAddressRepository.InsertAsync(personAddress, cancellationToken);
    }
}