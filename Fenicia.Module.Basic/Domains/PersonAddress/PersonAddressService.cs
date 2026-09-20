using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.DTOs.Basic.PersonAddress;
using Fenicia.Module.Basic.Domains.PersonAddress.Interfaces;

namespace Fenicia.Module.Basic.Domains.PersonAddress;

public sealed class PersonAddressService(IPersonAddressRepository repository) : IPersonAddressService
{
    public PersonAddressService()
        : this(null!)
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

        var result = await repository.InsertAsync(personAddress, cancellationToken);
        return new GetPersonAddressResponse(result.Id, result.PersonId, result.AddressId, result.Person.Name);
    }
}
