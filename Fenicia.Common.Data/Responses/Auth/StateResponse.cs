namespace Fenicia.Common.Data.Responses.Auth;

public class StateResponse
{
    public Guid Id
    {
        get; set;
    }

    public string Name { get; set; } = null!;

    public string Uf { get; set; } = null!;
}