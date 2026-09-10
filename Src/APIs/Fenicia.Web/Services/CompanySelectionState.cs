namespace Fenicia.Web.Services;

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