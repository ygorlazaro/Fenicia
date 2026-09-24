using System.ComponentModel;
using AwesomeAssertions;
using Fenicia.Common.Enums.Auth;

namespace Fenicia.Common.Tests.Enums;

public class OrderStatusEnumTests
{
    [Fact]
    public void OrderStatus_ShouldHaveExpectedValues()
    {
        var values = Enum.GetValues<EnumOrderStatus>();

        values.Should().Contain(EnumOrderStatus.Pending);
        values.Should().Contain(EnumOrderStatus.Approved);
        values.Should().Contain(EnumOrderStatus.Cancelled);
    }

    [Fact]
    public void OrderStatus_ShouldHaveCorrectCount()
    {
        var values = Enum.GetValues<EnumOrderStatus>();

        values.Length.Should().Be(3);
    }

    [Fact]
    public void OrderStatus_ShouldHaveDescriptionAttributes()
    {
        var pendingDescription = GetEnumDescription(EnumOrderStatus.Pending);
        var approvedDescription = GetEnumDescription(EnumOrderStatus.Approved);
        var cancelledDescription = GetEnumDescription(EnumOrderStatus.Cancelled);

        pendingDescription.Should().Be("Order is pending approval");
        approvedDescription.Should().Be("Order has been approved");
        cancelledDescription.Should().Be("Order has been cancelled");
    }

    private static string GetEnumDescription(EnumOrderStatus value)
    {
        var field = value.GetType().GetField(value.ToString())!;
        var attribute =
            field.GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault() as DescriptionAttribute;
        return attribute!.Description;
    }
}
