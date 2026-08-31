using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Person;

namespace Fenicia.Module.Basic.Domains.Person;

public class PersonService
{
    private readonly IPersonRepository _personRepository;

    public PersonService()
        : this(null!)
    {
    }

    public PersonService(IPersonRepository personRepository)
    {
        _personRepository = personRepository;
    }

    public virtual async Task<PersonModel> InsertAsync(PersonModel person, Guid companyId, CancellationToken cancellationToken = default)
    {
        person.CompanyId = companyId;
        return await _personRepository.InsertAsync(person, cancellationToken);
    }

    public virtual async Task<PersonModel?> UpdateAsync(Guid id, PersonModel person, Guid companyId, CancellationToken cancellationToken = default)
    {
        person.CompanyId = companyId;
        return await _personRepository.UpdateAsync(id, person, cancellationToken);
    }

    public virtual async Task<PersonModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _personRepository.GetByIdAsync(id, cancellationToken);
    }
}
