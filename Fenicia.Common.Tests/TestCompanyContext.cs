namespace Fenicia.Common.Data;

public record TestCompanyContext : ICompanyContext
{
    public Guid CompanyId { get; set; } = Guid.NewGuid();
}
