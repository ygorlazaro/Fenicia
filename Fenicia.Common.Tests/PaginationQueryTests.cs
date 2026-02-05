namespace Fenicia.Common.Tests;

[TestFixture]
public class PaginationQueryTests
{
    [Test]
    public void Defaults_Are_Page_1_PerPage_10()
    {
        var q = new PaginationQuery();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(q.Page, Is.EqualTo(1));
            Assert.That(q.PerPage, Is.EqualTo(10));
        }

    }
}
