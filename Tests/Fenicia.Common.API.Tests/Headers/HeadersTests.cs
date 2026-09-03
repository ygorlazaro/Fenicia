using AwesomeAssertions;

namespace Fenicia.Common.API.Tests.Headers;

public class HeadersTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithDefaultValues()
    {
        var headers = new API.Headers();

        headers.CompanyId.Should().Be(Guid.Empty);
    }

    [Fact]
    public void CompanyId_ShouldBeSettable()
    {
        var headers = new API.Headers();
        var companyId = Guid.NewGuid();

        headers.CompanyId = companyId;

        headers.CompanyId.Should().Be(companyId);
    }
}