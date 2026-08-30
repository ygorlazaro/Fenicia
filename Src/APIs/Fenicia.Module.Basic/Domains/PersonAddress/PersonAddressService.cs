using Fenicia.Common.Data.Models.Basic;

namespace Fenicia.Module.Basic.Domains.PersonAddress;

public class PersonAddressService(IPersonAddressRepository personAddressRepository)
{
    public async Task<PersonAddressModel> InsertAsync(PersonAddressModel personAddress, Guid companyId, CancellationToken ct)
    {
        personAddress.CompanyId = companyId;
        return await personAddressRepository.InsertAsync(personAddress, ct);
    }
}
