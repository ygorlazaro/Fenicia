using System.ComponentModel;
using Fenicia.Common.Enums.Auth;
using FluentAssertions;

namespace Fenicia.Common.Tests.Enums;

public class OrderStatusEnumTests
{
    [Fact]
    public void OrderStatus_ShouldHaveExpectedValues()
    {
        var values = Enum.GetValues<OrderStatus>();

        values.Should().Contain(OrderStatus.Pending);
        values.Should().Contain(OrderStatus.Approved);
        values.Should().Contain(OrderStatus.Cancelled);
    }

    [Fact]
    public void OrderStatus_ShouldHaveCorrectCount()
    {
        var values = Enum.GetValues<OrderStatus>();

        values.Length.Should().Be(3);
    }

    [Fact]
    public void OrderStatus_ShouldHaveDescriptionAttributes()
    {
        var pendingDescription = GetEnumDescription(OrderStatus.Pending);
        var approvedDescription = GetEnumDescription(OrderStatus.Approved);
        var cancelledDescription = GetEnumDescription(OrderStatus.Cancelled);

        pendingDescription.Should().Be("Order is pending approval");
        approvedDescription.Should().Be("Order has been approved");
        cancelledDescription.Should().Be("Order has been cancelled");
    }

    private static string GetEnumDescription(OrderStatus value)
    {
        var field = value.GetType().GetField(value.ToString())!;
        var attribute = field.GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault() as DescriptionAttribute;
        return attribute!.Description;
    }
}
