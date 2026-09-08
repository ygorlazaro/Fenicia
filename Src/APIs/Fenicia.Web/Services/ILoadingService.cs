namespace Fenicia.Web.Services;

public interface ILoadingService
{
    event Action<bool>? LoadingChanged;

    bool IsLoading { get; }

    void Increment();

    void Decrement();
}

public sealed class LoadingService : ILoadingService
{
    private int _count;

    public event Action<bool>? LoadingChanged;

    public bool IsLoading => _count > 0;

    public void Increment()
    {
        _count++;
        if (_count == 1)
        {
            LoadingChanged?.Invoke(true);
        }
    }

    public void Decrement()
    {
        if (_count > 0)
        {
            _count--;
        }

        if (_count == 0)
        {
            LoadingChanged?.Invoke(false);
        }
    }
}
