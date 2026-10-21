using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.DTOs.Auth.PersonAddress;

namespace Fenicia.Auth.Domains.PersonAddress;

/// <summary>
/// Mapper class for mapping between PersonAddressModel and PersonAddressResponse/PersonAddressRequest.
/// </summary>
public static class PersonAddressMapper
{
    /// <summary>
    /// Maps a PersonAddressModel to a PersonAddressResponse.
    /// </summary>
    /// <param name="pa">The person-address model.</param>
    /// <returns>The person-address response.</returns>
    public static PersonAddressResponse MapToPersonAddressResponse(PersonAddressModel pa)
    {
        return new PersonAddressResponse(
            pa.Id,
            pa.PersonId,
            pa.AddressId,
            pa.Person.Name);
    }

    /// <summary>
    /// Maps a PersonAddressRequest to a PersonAddressModel.
    /// </summary>
    /// <param name="request">The person-address request.</param>
    /// <returns>The person-address model.</returns>
    public static PersonAddressModel MapToPersonAddressModel(PersonAddressRequest request)
    {
        return new PersonAddressModel
        {
            Id = request.PersonId,
            PersonId = request.PersonId,
            AddressId = request.AddressId
        };
    }
}
