namespace Fenicia.Auth.Domains.State.Data;

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

    public string Uf
    {
        get; set;
    }

    public static StateResponse Convert(StateModel state)
    {
        return new StateResponse() { Id = state.Id, Name = state.Name, Uf = state.Uf };
    }

    public static List<StateResponse> Convert(List<StateModel> state)
    {
        return state.Select(StateResponse.Convert).ToList();
    }
}
