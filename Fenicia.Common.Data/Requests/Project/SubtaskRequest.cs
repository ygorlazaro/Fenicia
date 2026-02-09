namespace Fenicia.Common.Data.Requests.Project;

public class SubtaskRequest
{
    public Guid TaskId { get; set; }

    public string Title { get; set; } = string.Empty;

    public bool IsCompleted { get; set; }

    public int Order { get; set; }
}