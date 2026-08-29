namespace Fenicia.Common.API.Tests;

public class TestCompanyContext : Fenicia.Common.Data.ICompanyContext
{
    public Guid CompanyId { get; } = Guid.NewGuid();
}
