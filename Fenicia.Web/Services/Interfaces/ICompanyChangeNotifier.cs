namespace Fenicia.Web.Services.Interfaces;

public interface ICompanyChangeNotifier
{
    event Action? Changed;

    void Notify();
}
