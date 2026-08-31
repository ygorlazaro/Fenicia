using Fenicia.Common.Data.Models.Basic;

namespace Fenicia.Module.Basic.Domains.PersonAddress;

public class PersonAddressService
{
    private readonly IPersonAddressRepository _personAddressRepository;

    public PersonAddressService()
        : this(null!)
    {
    }

    public PersonAddressService(IPersonAddressRepository personAddressRepository)
    {
        _personAddressRepository = personAddressRepository;
    }

    public virtual async Task<PersonAddressModel> InsertAsync(PersonAddressModel personAddress, Guid companyId, CancellationToken cancellationToken = default)
    {
        personAddress.CompanyId = companyId;
        return await _personAddressRepository.InsertAsync(personAddress, cancellationToken);
    }
}
