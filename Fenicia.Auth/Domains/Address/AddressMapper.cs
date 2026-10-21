using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.DTOs.Auth.Address;

namespace Fenicia.Auth.Domains.Address;

/// <summary>
/// Mapper class for mapping between AddressModel and AddressResponse/AddressRequest.
/// </summary>
public static class AddressMapper
{
    /// <summary>
    /// Maps an AddressModel to an AddressResponse.
    /// </summary>
    /// <param name="address">The address model.</param>
    /// <returns>The address response.</returns>
    public static AddressResponse MapToAddressResponse(AddressModel address)
    {
        return new AddressResponse(
            address.Id,
            address.Street,
            address.Number,
            address.Complement,
            address.Neighborhood,
            address.ZipCode,
            address.StateId,
            address.State?.Name,
            address.City,
            address.Country);
    }

    /// <summary>
    /// Maps an AddressRequest to an AddressModel.
    /// </summary>
    /// <param name="request">The address request.</param>
    /// <returns>The address model.</returns>
    public static AddressModel MapToAddressModel(AddressRequest request)
    {
        return new AddressModel
        {
            Id = Guid.NewGuid(),
            Street = request.Street,
            Number = request.Number,
            Complement = request.Complement,
            Neighborhood = request.Neighborhood,
            ZipCode = request.ZipCode,
            StateId = request.StateId,
            City = request.City,
            Country = request.Country,
            IsDefault = true
        };
    }
}
