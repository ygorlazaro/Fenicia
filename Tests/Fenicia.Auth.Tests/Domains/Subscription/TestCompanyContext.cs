using Fenicia.Common.Data;

namespace Fenicia.Auth.Tests.Domains.Subscription;

internal sealed class TestCompanyContext : ICompanyContext
{
    public Guid CompanyId { get; set; }
}
