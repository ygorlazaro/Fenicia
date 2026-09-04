using MudBlazor;

namespace Fenicia.Web.Services;

public class NotificationService
{
    private readonly List<NotificationItem> _notifications = [];
    private int _idCounter = 0;

    public event Action? OnChange;

    public IReadOnlyList<NotificationItem> Notifications => _notifications;

    public void Show(string message, Severity severity, int durationMs = 5000)
    {
        var id = ++_idCounter;
        var item = new NotificationItem(id, message, severity);
        _notifications.Add(item);
        NotifyStateChanged();

        _ = Task.Run(async () =>
        {
            await Task.Delay(durationMs);
            Remove(id);
        });
    }

    public void Remove(int id)
    {
        var item = _notifications.FirstOrDefault(n => n.Id == id);
        if (item != null)
        {
            _notifications.Remove(item);
            NotifyStateChanged();
        }
    }

    public void Clear()
    {
        _notifications.Clear();
        NotifyStateChanged();
    }

    private void NotifyStateChanged()
    {
        OnChange?.Invoke();
    }
}
