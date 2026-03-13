using Fenicia.Common.Data;

namespace Fenicia.Common.Tests;

public record TestCompanyContext : ICompanyContext
{
    public Guid CompanyId { get; set; } = Guid.NewGuid();
}
