namespace Fenicia.Common.Data;

public class TestCompanyContext : ICompanyContext
{
    public Guid? CompanyId { get; set; } = Guid.NewGuid();
}