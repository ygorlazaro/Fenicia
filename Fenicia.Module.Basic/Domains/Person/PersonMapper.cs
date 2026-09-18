using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.DTOs.Basic.Person;
using Riok.Mapperly.Abstractions;

namespace Fenicia.Module.Basic.Domains.Person;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class PersonMapper
{
    public partial GetPersonByIdResponse MapToGetPersonByIdResponse(PersonModel person);
}
