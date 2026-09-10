namespace Fenicia.Web.Services;

public interface IUserProfileNotifier
{
    event Action? Changed;

    void Notify();
}