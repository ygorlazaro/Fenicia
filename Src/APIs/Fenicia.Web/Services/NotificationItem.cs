using MudBlazor;

namespace Fenicia.Web.Services;

public class NotificationItem(int id, string message, Severity severity)
{
    public int Id { get; } = id;

    public string Message { get; } = message;

    public Severity Severity { get; } = severity;

    public DateTime CreatedAt { get; } = DateTime.Now;
}
