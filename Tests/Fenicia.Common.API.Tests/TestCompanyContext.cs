using Fenicia.Common.Data;

namespace Fenicia.Common.API.Tests;

public class TestCompanyContext : ICompanyContext
{
    public Guid CompanyId { get; } = Guid.NewGuid();
}