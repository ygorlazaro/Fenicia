namespace Fenicia.Common.Enums;

using System.ComponentModel;

public enum OrderStatus
{
    [Description("Order is pending approval")]
    Pending = 0,

    [Description("Order has been approved")]
    Approved = 1,

    [Description("Order has been cancelled")]
    Cancelled = 2
}
