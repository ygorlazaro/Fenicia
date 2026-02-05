namespace Fenicia.Common.Data.Responses.Auth;

public class StateResponse
{
    public Guid Id
    {
        get; set;
    }

    public string Name { get; set; } = default!;

    public string Uf { get; set; } = default!;
}
