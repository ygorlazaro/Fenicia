namespace Fenicia.Web.Services;

public class UserProfileNotifier : IUserProfileNotifier
{
    public event Action? Changed;

    public void Notify()
    {
        Changed?.Invoke();
    }
}
