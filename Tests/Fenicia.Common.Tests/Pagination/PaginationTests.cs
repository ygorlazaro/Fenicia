using AwesomeAssertions;

namespace Fenicia.Common.Tests.Pagination;

public class PaginationTests
{
    [Fact]
    public void Constructor_ShouldInitializeProperties()
    {
        var data = new List<int> { 1, 2, 3 };
        var pagination = new Pagination<List<int>>(data, 3, 1, 10);

        pagination.Data.Should().BeEquivalentTo(data);
        pagination.Total.Should().Be(3);
        pagination.Page.Should().Be(1);
        pagination.PerPage.Should().Be(10);
    }

    [Fact]
    public void Constructor_WithPaginationQuery_ShouldInitializeProperties()
    {
        var data = new List<int> { 1, 2, 3 };
        var query = new PaginationQuery(2, 5);
        var pagination = new Pagination<List<int>>(data, 3, query);

        pagination.Data.Should().BeEquivalentTo(data);
        pagination.Total.Should().Be(3);
        pagination.Page.Should().Be(2);
        pagination.PerPage.Should().Be(5);
    }

    [Fact]
    public void Pages_ShouldCalculateCorrectly()
    {
        var pagination = new Pagination<List<int>>(new List<int>(), 10, 1, 3);

        pagination.Pages.Should().Be(4);
    }

    [Fact]
    public void Pages_WhenTotalIsZero_ShouldReturnZero()
    {
        var pagination = new Pagination<List<int>>(new List<int>(), 0, 1, 10);

        pagination.Pages.Should().Be(0);
    }

    [Fact]
    public void Pages_WhenTotalIsExactMultiple_ShouldReturnExactPages()
    {
        var pagination = new Pagination<List<int>>(new List<int>(), 10, 1, 5);

        pagination.Pages.Should().Be(2);
    }
}
