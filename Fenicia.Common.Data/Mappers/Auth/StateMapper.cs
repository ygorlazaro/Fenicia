using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Responses.Auth;

namespace Fenicia.Common.Data.Mappers.Auth;

public static class StateMapper
{
    public static StateResponse Map(StateModel state)
    {
        return new StateResponse { Id = state.Id, Name = state.Name, Uf = state.Uf };
    }

    public static List<StateResponse> Map(List<StateModel> state)
    {
        return [.. state.Select(Map)];
    }
}
