namespace Fenicia.Web.Services;

public interface ICompanySelectionState
{
    Guid? SelectedCompanyId { get; }

    string SelectedCompanyName { get; }

    void Set(Guid companyId, string companyName);
}

public class CompanySelectionState : ICompanySelectionState
{
    public Guid? SelectedCompanyId { get; private set; }

    public string SelectedCompanyName { get; private set; } = string.Empty;

    public void Set(Guid companyId, string companyName)
    {
        SelectedCompanyId = companyId;
        SelectedCompanyName = companyName;
    }
}