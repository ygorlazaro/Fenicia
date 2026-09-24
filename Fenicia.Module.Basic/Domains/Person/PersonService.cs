using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.DTOs.Auth.Person;
using Fenicia.Module.Basic.Domains.Person.Interfaces;

namespace Fenicia.Module.Basic.Domains.Person;

public sealed class PersonService(IPersonRepository repository) : IPersonService
{
    public PersonService()
        : this(null!)
    {
    }

    public async Task<PersonResponse> InsertAsync(
        PersonRequest command,
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

        var result = await repository.InsertAsync(person, cancellationToken);
        return new PersonResponse(
            result.Id,
            result.Name,
            result.Document,
            result.Email,
            result.PhoneNumber,
            result.DateOfBirth,
            result.PhotoUrl,
            result.Notes);
    }

    public async Task<PersonResponse?> UpdateAsync(
        Guid id,
        PersonRequest command,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        var person = await repository.GetByIdAsync(id, cancellationToken);
        if (person is null)
        {
            return null;
        }

        person.Name = command.Name;
        person.Document = command.Document;
        person.Email = command.Email;
        person.PhoneNumber = command.PhoneNumber;
        person.CompanyId = companyId;
        var result = await repository.UpdateAsync(id, person, cancellationToken);
        return result is null ? null : new PersonResponse(
            result.Id,
            result.Name,
            result.Document,
            result.Email,
            result.PhoneNumber,
            result.DateOfBirth,
            result.PhotoUrl,
            result.Notes);
    }
}
