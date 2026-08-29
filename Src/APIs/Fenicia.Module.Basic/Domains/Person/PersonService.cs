using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Person;

namespace Fenicia.Module.Basic.Domains.Person;

public class PersonService(PersonRepository personRepository)
{
    public PersonService()
        : this(null!)
    {
    }

    public async Task<PersonModel> InsertAsync(PersonModel person, Guid companyId, CancellationToken ct)
    {
        person.CompanyId = companyId;
        return await personRepository.InsertAsync(person, ct);
    }

    public async Task<PersonModel?> UpdateAsync(Guid id, PersonModel person, Guid companyId, CancellationToken ct)
    {
        person.CompanyId = companyId;
        return await personRepository.UpdateAsync(id, person, ct);
    }

    public async Task<PersonModel?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await personRepository.GetByIdAsync(id, ct);
    }
}
