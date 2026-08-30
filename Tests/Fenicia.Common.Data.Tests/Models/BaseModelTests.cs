using AwesomeAssertions;
using Fenicia.Common.Data;

namespace Fenicia.Common.Data.Tests.Models;

public class BaseModelTests
{
    [Fact]
    public void Constructor_ShouldGenerateNewId()
    {
        var model = new TestBaseModel();

        model.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Constructor_ShouldSetCreatedToUtcNow()
    {
        var before = DateTime.UtcNow;
        var model = new TestBaseModel();
        var after = DateTime.UtcNow;

        model.Created.Should().BeOnOrAfter(before);
        model.Created.Should().BeOnOrBefore(after);
    }

    [Fact]
    public void Constructor_ShouldInitializeUpdatedAndDeletedAsNull()
    {
        var model = new TestBaseModel();

        model.Updated.Should().BeNull();
        model.Deleted.Should().BeNull();
    }

    private sealed class TestBaseModel : BaseModel
    {
    }
}
