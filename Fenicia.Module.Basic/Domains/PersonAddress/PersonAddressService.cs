using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.DTOs.Basic.PersonAddress;
using Fenicia.Module.Basic.Domains.PersonAddress.Interfaces;

namespace Fenicia.Module.Basic.Domains.PersonAddress;

public sealed class PersonAddressService(IPersonAddressRepository personAddressRepository, PersonAddressMapper personAddressMapper) : IPersonAddressService
{
    public PersonAddressService()
        : this(null!, null!)
    {
    }

    public async Task<GetPersonAddressResponse> InsertAsync(
        AddPersonAddressCommand command,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        var personAddress = new PersonAddressModel
        {
            Id = Guid.NewGuid(),
            PersonId = command.PersonId,
            AddressId = command.AddressId,
            CompanyId = companyId
        };

        var result = await personAddressRepository.InsertAsync(personAddress, cancellationToken);
        return personAddressMapper.MapToGetPersonAddressResponse(result);
    }
}
