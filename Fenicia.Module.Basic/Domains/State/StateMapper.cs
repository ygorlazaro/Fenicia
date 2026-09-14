using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.DTOs.Basic.State;
using Riok.Mapperly.Abstractions;

namespace Fenicia.Module.Basic.Domains.State;

[Mapper]
public static partial class StateMapper
{
    public static GetAllStateResponse MapToGetAllStateResponse(this StateModel state)
    {
        return new GetAllStateResponse(state.Id, state.Name, state.Uf);
    }
}