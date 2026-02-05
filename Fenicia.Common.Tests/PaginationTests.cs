namespace Fenicia.Common.Tests;

[TestFixture]
public class PaginationTests
{
    [Test]
    public void Pagination_Primary_Constructor_Sets_Properties_And_Pages()
    {
        var data = new List<int> { 1, 2, 3 };
        const int total = 25;
        const int page = 2;
        const int perPage = 10;

        var p = new Pagination<List<int>>(data, total, page, perPage);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(p.Data, Is.EqualTo(data));
            Assert.That(p.Total, Is.EqualTo(total));
            Assert.That(p.Page, Is.EqualTo(page));
            Assert.That(p.PerPage, Is.EqualTo(perPage));
            Assert.That(p.Pages, Is.EqualTo(3));
        }

    }

    [Test]
    public void Pagination_Constructor_With_Query_Works()
    {
        var data = new[] { "a", "b" };
        const int total = 5;
        var query = new PaginationQuery { Page = 1, PerPage = 2 };

        var p = new Pagination<string[]>(data, total, query);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(p.Data, Is.EqualTo(data));
            Assert.That(p.Total, Is.EqualTo(total));
            Assert.That(p.Page, Is.EqualTo(query.Page));
            Assert.That(p.PerPage, Is.EqualTo(query.PerPage));
            Assert.That(p.Pages, Is.EqualTo(3));
        }

    }
}
