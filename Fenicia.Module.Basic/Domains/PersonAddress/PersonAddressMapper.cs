using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.DTOs.Basic.PersonAddress;
using Riok.Mapperly.Abstractions;

namespace Fenicia.Module.Basic.Domains.PersonAddress;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class PersonAddressMapper
{
    [MapProperty("Person.Name", nameof(GetPersonAddressResponse.PersonName))]
    public partial GetPersonAddressResponse MapToGetPersonAddressResponse(PersonAddressModel personAddress);
}
