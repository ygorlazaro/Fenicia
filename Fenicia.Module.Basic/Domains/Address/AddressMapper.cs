using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.DTOs.Basic.Address;
using Riok.Mapperly.Abstractions;

namespace Fenicia.Module.Basic.Domains.Address;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.None)]
public partial class AddressMapper
{
    [MapProperty("State.Name", nameof(AddressResponse.StateName))]
    public partial AddressResponse MapToAddressResponse(AddressModel address);
}
