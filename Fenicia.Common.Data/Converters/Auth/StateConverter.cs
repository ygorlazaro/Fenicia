using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Responses.Auth;

namespace Fenicia.Common.Data.Converters.Auth;

public static class StateConverter
{
    public static StateResponse Convert(StateModel state)
    {
        return new StateResponse { Id = state.Id, Name = state.Name, Uf = state.Uf };
    }

    public static List<StateResponse> Convert(List<StateModel> state)
    {
        return [.. state.Select(Convert)];
    }
}
