namespace Fenicia.Auth.Enums;

using System.ComponentModel;

public enum OrderStatus
{
    [Description(description: "Order is pending approval")]
    Pending = 0,

    [Description(description: "Order has been approved")]
    Approved = 1,

    [Description(description: "Order has been cancelled")]
    Cancelled = 2
}
