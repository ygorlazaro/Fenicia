namespace Fenicia.Web.Services;

public interface ICompanyChangeNotifier
{
    event Action? Changed;

    void Notify();
}