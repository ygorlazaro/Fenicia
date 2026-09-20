using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.DTOs.Basic.Person;
using Fenicia.Module.Basic.Domains.Person.Interfaces;

namespace Fenicia.Module.Basic.Domains.Person;

public sealed class PersonService(IPersonRepository personRepository, PersonMapper personMapper) : IPersonService
{
    public PersonService()
        : this(null!, null!)
    {
    }

    public async Task<GetPersonByIdResponse> InsertAsync(
        UpsertPersonCommand command,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        var person = new PersonModel
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            Document = command.Document,
            Email = command.Email,
            PhoneNumber = command.PhoneNumber,
            DateOfBirth = command.DateOfBirth,
            PhotoUrl = command.PhotoUrl,
            Notes = command.Notes,
            CompanyId = companyId
        };

        var result = await personRepository.InsertAsync(person, cancellationToken);
        return personMapper.MapToGetPersonByIdResponse(result);
    }

    public async Task<GetPersonByIdResponse?> UpdateAsync(
        Guid id,
        UpsertPersonCommand command,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        var person = await personRepository.GetByIdAsync(id, cancellationToken);
        if (person is null)
        {
            return null;
        }

        person.Name = command.Name;
        person.Document = command.Document;
        person.Email = command.Email;
        person.PhoneNumber = command.PhoneNumber;
        person.CompanyId = companyId;
        var result = await personRepository.UpdateAsync(id, person, cancellationToken);
        return result is null ? null : personMapper.MapToGetPersonByIdResponse(result);
    }
}
