namespace Fenicia.Common.Data.Requests.Project;

public class StatusRequest
{
    public Guid ProjectId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Color { get; set; } = string.Empty;

    public int Order { get; set; }

    public bool IsFinal { get; set; }
}