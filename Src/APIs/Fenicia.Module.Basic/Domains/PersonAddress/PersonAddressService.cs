using Fenicia.Common.Data.Models.Basic;

namespace Fenicia.Module.Basic.Domains.PersonAddress;

public class PersonAddressService(PersonAddressRepository personAddressRepository)
{
    public PersonAddressService()
        : this(null!)
    {
    }

    public async Task<PersonAddressModel> InsertAsync(PersonAddressModel personAddress, Guid companyId, CancellationToken ct)
    {
        personAddress.CompanyId = companyId;
        return await personAddressRepository.InsertAsync(personAddress, ct);
    }
}
