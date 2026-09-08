namespace Fenicia.Web.Services;

public interface IUserProfileNotifier
{
    event Action? Changed;

    void Notify();
}

public class UserProfileNotifier : IUserProfileNotifier
{
    public event Action? Changed;

    public void Notify()
    {
        Changed?.Invoke();
    }
}
