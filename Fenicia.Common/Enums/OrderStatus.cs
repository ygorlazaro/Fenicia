using System.ComponentModel;

namespace Fenicia.Common.Enums;

public enum OrderStatus
{
    [Description("Order is pending approval")]
    Pending = 0,

    [Description("Order has been approved")]
    Approved = 1,

    [Description("Order has been cancelled")]
    Cancelled = 2
}
