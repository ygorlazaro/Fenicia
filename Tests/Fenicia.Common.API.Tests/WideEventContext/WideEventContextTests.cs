using System.Diagnostics;
using AwesomeAssertions;
using Fenicia.Common.API;

namespace Fenicia.Common.API.Tests.WideEventContext;

public class WideEventContextTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithDefaultValues()
    {
        var wide = new Fenicia.Common.API.WideEventContext();

        wide.Path.Should().BeNull();
        wide.Method.Should().BeNull();
        wide.StatusCode.Should().Be(0);
        wide.DurationMs.Should().Be(0);
        wide.UserId.Should().BeNull();
        wide.Success.Should().BeFalse();
        wide.ErrorCode.Should().BeNull();
        wide.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public void Operation_ShouldCombinePathAndMethod()
    {
        var wide = new Fenicia.Common.API.WideEventContext
        {
            Path = "/api/users",
            Method = "GET"
        };

        wide.Operation.Should().Be("/api/users GET");
    }

    [Fact]
    public void TraceId_ShouldBeGeneratedFromActivity()
    {
        using var activity = new Activity("TestActivity").Start();

        var wide = new Fenicia.Common.API.WideEventContext();

        wide.TraceId.Should().NotBeEmpty();
        wide.TraceId.Should().MatchRegex(@"^[0-9a-f]{32}$");
    }
}
