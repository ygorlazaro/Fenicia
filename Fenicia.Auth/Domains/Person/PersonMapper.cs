using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.DTOs.Auth.Person;

namespace Fenicia.Auth.Domains.Person;

/// <summary>
/// Mapper class for mapping between PersonModel and PersonResponse/PersonRequest.
/// </summary>
public static class PersonMapper
{
    /// <summary>
    /// Maps a PersonModel to a PersonResponse.
    /// </summary>
    /// <param name="person">The person model.</param>
    /// <returns>The person response.</returns>
    public static PersonResponse MapToPersonResponse(PersonModel person)
    {
        return new PersonResponse(
            person.Id,
            person.Name,
            person.Document,
            person.Email,
            person.PhoneNumber,
            person.DateOfBirth,
            person.PhotoUrl,
            null);
    }

    /// <summary>
    /// Maps a PersonRequest to a PersonModel.
    /// </summary>
    /// <param name="request">The person request.</param>
    /// <returns>The person model.</returns>
    public static PersonModel MapToPersonModel(PersonRequest request)
    {
        return new PersonModel
        {
            Id = request.Id,
            Name = request.Name,
            Document = request.Document,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            DateOfBirth = request.DateOfBirth,
            PhotoUrl = request.PhotoUrl
        };
    }
}
