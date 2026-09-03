using AwesomeAssertions;

namespace Fenicia.Common.Tests.Pagination;

public class PaginationQueryTests
{
    [Fact]
    public void Constructor_ShouldInitializeProperties()
    {
        var query = new PaginationQuery();

        query.Page.Should().Be(1);
        query.PerPage.Should().Be(10);
    }

    [Fact]
    public void Equality_ShouldWork()
    {
        var query1 = new PaginationQuery();
        var query2 = new PaginationQuery();

        query1.Should().Be(query2);
    }

    [Fact]
    public void Inequality_ShouldWork()
    {
        var query1 = new PaginationQuery();
        var query2 = new PaginationQuery(2);

        query1.Should().NotBe(query2);
    }
}