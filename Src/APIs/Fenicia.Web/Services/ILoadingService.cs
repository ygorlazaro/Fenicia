namespace Fenicia.Web.Services;

public interface ILoadingService
{
    event Action<bool>? LoadingChanged;

    bool IsLoading { get; }

    void Increment();

    void Decrement();
}