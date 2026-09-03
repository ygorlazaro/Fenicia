using Fenicia.Common.Data;

namespace Fenicia.Common.Tests;

public class TestCompanyContext : ICompanyContext
{
    public Guid CompanyId { get; } = Guid.NewGuid();
}