using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Person.Interfaces;

namespace Fenicia.Module.Basic.Domains.Person;

public sealed class PersonService(IPersonRepository personRepository) : IPersonService
{
    public PersonService()
        : this(null!)
    {
    }

    public Task<PersonModel> InsertAsync(
        PersonModel person,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        person.CompanyId = companyId;
        return personRepository.InsertAsync(person, cancellationToken);
    }

    public Task<PersonModel?> UpdateAsync(
        Guid id,
        PersonModel person,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        person.CompanyId = companyId;
        return personRepository.UpdateAsync(id, person, cancellationToken);
    }

    public Task<PersonModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return personRepository.GetByIdAsync(id, cancellationToken);
    }
}