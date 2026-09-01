using Fenicia.Common.Data.Models.Auth;
using Fenicia.Module.Basic.Domains.Address.DTOs;
using Riok.Mapperly.Abstractions;

namespace Fenicia.Module.Basic.Domains.Address;

[Mapper]
public static partial class AddressMapper
{
    public static AddressResponse MapToAddressResponse(this AddressModel address)
    {
        return new AddressResponse(address.Id, address.Street, address.Number, address.Complement, address.Neighborhood, address.ZipCode!, address.StateId, address.State.Name, address.City, address.Country);
    }
}
