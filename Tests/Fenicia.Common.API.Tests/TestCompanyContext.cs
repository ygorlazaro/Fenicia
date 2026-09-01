namespace Fenicia.Common.API.Tests;

public class TestCompanyContext : Data.ICompanyContext
{
    public Guid CompanyId { get; } = Guid.NewGuid();
}
