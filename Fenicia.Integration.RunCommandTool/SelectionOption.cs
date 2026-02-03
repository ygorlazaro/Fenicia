namespace Fenicia.Integration.RunCommandTool;

public record SelectionOption(int id, string description)
{
    public void Deconstruct(out int id, out string description)
    {
        id = this.id;
        description = this.description;
    }
}
