namespace Fenicia.Web.Services.Interfaces;

public interface IUserProfileNotifier
{
    event Action? Changed;

    void Notify();
}
