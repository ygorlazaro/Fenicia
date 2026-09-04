using BlazorExpress.Bulma;

namespace Fenicia.Web.Services;

public class NotificationService
{
    private string? _message;
    private NotificationColor _color = NotificationColor.Info;
    private bool _isVisible;

    public string? Message
    {
        get => _message;
        set
        {
            _message = value;
            NotifyStateChanged();
        }
    }

    public NotificationColor Color
    {
        get => _color;
        set
        {
            _color = value;
            NotifyStateChanged();
        }
    }

    public bool IsVisible
    {
        get => _isVisible;
        set
        {
            _isVisible = value;
            NotifyStateChanged();
        }
    }

    public event Action? OnChange;

    public void Show(string message, NotificationColor color, int durationMs = 5000)
    {
        Message = message;
        Color = color;
        IsVisible = true;
        NotifyStateChanged();

        _ = Task.Run(async () =>
        {
            await Task.Delay(durationMs);
            IsVisible = false;
            Message = null;
            NotifyStateChanged();
        });
    }

    private void NotifyStateChanged()
    {
        OnChange?.Invoke();
    }
}
