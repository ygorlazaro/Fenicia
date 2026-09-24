using AwesomeAssertions;
using Fenicia.Common.Enums.Auth;

namespace Fenicia.Common.Tests.Enums;

public class SubscriptionStatusEnumTests
{
    [Fact]
    public void SubscriptionStatus_ShouldHaveExpectedValues()
    {
        var values = Enum.GetValues<EnumSubscriptionStatus>();

        values.Should().Contain(EnumSubscriptionStatus.Inactive);
        values.Should().Contain(EnumSubscriptionStatus.Active);
    }

    [Fact]
    public void SubscriptionStatus_ShouldHaveCorrectCount()
    {
        var values = Enum.GetValues<EnumSubscriptionStatus>();

        values.Length.Should().Be(2);
    }
}
