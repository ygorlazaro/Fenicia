namespace Fenicia.Web.Models;

public class ModuleResponse
{
    public Guid Id
    {
        get; set;
    }
    public string Name { get; set; } = string.Empty;
    public decimal Amount
    {
        get; set;
    }
}
