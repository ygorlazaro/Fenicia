namespace Fenicia.Auth.Enums;

using System.ComponentModel;

/// <summary>
///     Represents the current status of an order in the system.
/// </summary>
public enum OrderStatus
{
    /// <summary>
    ///     The order is awaiting approval or processing.
    /// </summary>
    [Description(description: "Order is pending approval")]
    Pending = 0,

    /// <summary>
    ///     The order has been approved and is being processed.
    /// </summary>
    [Description(description: "Order has been approved")]
    Approved = 1,

    /// <summary>
    ///     The order has been cancelled and will not be processed.
    /// </summary>
    [Description(description: "Order has been cancelled")]
    Cancelled = 2
}
