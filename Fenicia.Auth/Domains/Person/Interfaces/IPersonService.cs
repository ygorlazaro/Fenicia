using Fenicia.Common.DTOs.Auth.Person;

namespace Fenicia.Auth.Domains.Person.Interfaces;

/// <summary>
/// Defines the contract for a service that manages person-related operations.
/// </summary>
public interface IPersonService
{
    /// <summary>
    /// Inserts a new person.
    /// </summary>
    /// <param name="command">The person request containing the person details.</param>
    /// <param name="companyId">The unique identifier of the company.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The created person response.</returns>
    Task<PersonResponse> InsertAsync(PersonRequest command, Guid companyId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing person.
    /// </summary>
    /// <param name="id">The unique identifier of the person to update.</param>
    /// <param name="command">The person request containing the updated person details.</param>
    /// <param name="companyId">The unique identifier of the company.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The updated person response, or null if not found.</returns>
    Task<PersonResponse?> UpdateAsync(
        Guid id,
        PersonRequest command,
        Guid companyId,
        CancellationToken cancellationToken = default);
}