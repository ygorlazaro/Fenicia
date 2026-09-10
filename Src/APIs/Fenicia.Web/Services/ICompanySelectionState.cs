namespace Fenicia.Web.Services;

public interface ICompanySelectionState
{
    Guid? SelectedCompanyId { get; }

    string SelectedCompanyName { get; }

    void Set(Guid companyId, string companyName);
}