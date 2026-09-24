using Fenicia.Auth.Domains.Person.Interfaces;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.DTOs.Auth.Person;

namespace Fenicia.Auth.Domains.Person;

/// <summary>
/// Service responsible for managing person-related operations.
/// </summary>
/// <param name="repository">The repository for managing person data.</param>
public sealed class PersonService(IPersonRepository repository) : IPersonService
{
    /// <summary>
    /// Inserts a new person.
    /// </summary>
    /// <param name="command">The person request containing the person details.</param>
    /// <param name="companyId">The unique identifier of the company.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The created person response.</returns>
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

    /// <summary>
    /// Updates an existing person.
    /// </summary>
    /// <param name="id">The unique identifier of the person to update.</param>
    /// <param name="command">The person request containing the updated person details.</param>
    /// <param name="companyId">The unique identifier of the company.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The updated person response, or null if not found.</returns>
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