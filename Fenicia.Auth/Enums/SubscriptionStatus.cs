namespace Fenicia.Auth.Enums;

/// <summary>
///     Represents the current status of a subscription in the system.
/// </summary>
/// <remarks>
///     This enum is used to track whether a subscription is currently active or inactive.
///     The values are explicitly set to ensure database consistency.
/// </remarks>
public enum SubscriptionStatus
{
    /// <summary>
    ///     Indicates that the subscription is not currently active.
    ///     This status is used when a subscription has expired, been cancelled, or not yet activated.
    /// </summary>
    Inactive = 0,

    /// <summary>
    ///     Indicates that the subscription is currently active and valid.
    ///     This status is used for subscriptions that are paid and in good standing.
    /// </summary>
    Active = 1
}
