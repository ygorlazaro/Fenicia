namespace Fenicia.Web.Services;

public class CompanyChangeNotifier : ICompanyChangeNotifier
{
    public event Action? Changed;

    public void Notify()
    {
        Changed?.Invoke();
    }
}