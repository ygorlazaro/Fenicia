namespace Fenicia.Common.Database.Responses;

using Models.Auth;

public class StateResponse
{
    public Guid Id
    {
        get; set;
    }

    public string Name
    {
        get; set;
    }

= default!;

    public string Uf
    {
        get; set;
    }

= default!;

    public static StateResponse Convert(StateModel state)
    {
        return new StateResponse { Id = state.Id, Name = state.Name, Uf = state.Uf };
    }

    public static List<StateResponse> Convert(List<StateModel> state)
    {
        return [.. state.Select(StateResponse.Convert)];
    }
}
