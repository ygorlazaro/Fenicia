using BlazorExpress.Bulma;

namespace Fenicia.Web.Services;

public class NotificationService
{
    private readonly List<NotificationItem> _notifications = [];
    private int _idCounter = 0;

    public event Action? OnChange;

    public IReadOnlyList<NotificationItem> Notifications => _notifications;

    public void Show(string message, NotificationColor color, int durationMs = 5000)
    {
        var id = ++_idCounter;
        var item = new NotificationItem(id, message, color);
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

    public class NotificationItem(int id, string message, NotificationColor color)
    {
        public int Id { get; } = id;

        public string Message { get; } = message;

        public NotificationColor Color { get; } = color;

        public DateTime CreatedAt { get; } = DateTime.Now;
    }
}
