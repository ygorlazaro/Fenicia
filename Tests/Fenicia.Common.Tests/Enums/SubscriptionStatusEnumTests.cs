using AwesomeAssertions;
using Fenicia.Common.Enums.Auth;

namespace Fenicia.Common.Tests.Enums;

public class SubscriptionStatusEnumTests
{
    [Fact]
    public void SubscriptionStatus_ShouldHaveExpectedValues()
    {
        var values = Enum.GetValues<SubscriptionStatus>();

        values.Should().Contain(SubscriptionStatus.Inactive);
        values.Should().Contain(SubscriptionStatus.Active);
    }

    [Fact]
    public void SubscriptionStatus_ShouldHaveCorrectCount()
    {
        var values = Enum.GetValues<SubscriptionStatus>();

        values.Length.Should().Be(2);
    }
}