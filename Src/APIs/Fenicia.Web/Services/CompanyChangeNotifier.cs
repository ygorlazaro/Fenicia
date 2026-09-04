namespace Fenicia.Web.Services;

public interface ICompanyChangeNotifier
{
    event Action? Changed;

    void Notify();
}

public class CompanyChangeNotifier : ICompanyChangeNotifier
{
    public event Action? Changed;

    public void Notify()
    {
        Changed?.Invoke();
    }
}