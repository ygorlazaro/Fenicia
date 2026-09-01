using Fenicia.Common.Data.Models.Basic;

namespace Fenicia.Module.Basic.Domains.Person;

public class PersonService(IPersonRepository personRepository)
{
    public PersonService()
        : this(null!)
    {
    }

    public virtual async Task<PersonModel> InsertAsync(PersonModel person, Guid companyId, CancellationToken cancellationToken = default)
    {
        person.CompanyId = companyId;
        return await personRepository.InsertAsync(person, cancellationToken);
    }

    public virtual async Task<PersonModel?> UpdateAsync(Guid id, PersonModel person, Guid companyId, CancellationToken cancellationToken = default)
    {
        person.CompanyId = companyId;
        return await personRepository.UpdateAsync(id, person, cancellationToken);
    }

    public virtual async Task<PersonModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await personRepository.GetByIdAsync(id, cancellationToken);
    }
}
