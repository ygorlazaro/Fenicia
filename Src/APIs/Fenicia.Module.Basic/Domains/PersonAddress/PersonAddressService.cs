using Fenicia.Common.Data.Models.Basic;

namespace Fenicia.Module.Basic.Domains.PersonAddress;

public class PersonAddressService(IPersonAddressRepository personAddressRepository)
{
    public PersonAddressService()
        : this(null!)
    {
    }

    public virtual async Task<PersonAddressModel> InsertAsync(PersonAddressModel personAddress, Guid companyId, CancellationToken cancellationToken = default)
    {
        personAddress.CompanyId = companyId;
        return await personAddressRepository.InsertAsync(personAddress, cancellationToken);
    }
}
