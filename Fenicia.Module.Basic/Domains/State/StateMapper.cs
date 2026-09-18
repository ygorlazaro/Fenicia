using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.DTOs.Basic.State;
using Riok.Mapperly.Abstractions;

namespace Fenicia.Module.Basic.Domains.State;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class StateMapper
{
    public partial GetAllStateResponse MapToGetAllStateResponse(StateModel state);
}
