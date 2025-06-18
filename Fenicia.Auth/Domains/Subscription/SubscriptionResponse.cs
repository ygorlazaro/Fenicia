using Fenicia.Auth.Enums;

namespace Fenicia.Auth.Domains.Subscription;

/// <summary>
/// Response model containing subscription information
/// </summary>
public class SubscriptionResponse
{
    /// <summary>
    /// The unique identifier of the subscription
    /// </summary>
    /// <example>550e8400-e29b-41d4-a716-446655440000</example>
    public Guid Id { get; set; }

    /// <summary>
    /// The current status of the subscription
    /// </summary>
    public SubscriptionStatus Status { get; set; }

    /// <summary>
    /// The start date of the subscription
    /// </summary>
    /// <example>2025-06-03T00:00:00Z</example>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// The end date of the subscription
    /// </summary>
    /// <example>2026-06-03T00:00:00Z</example>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// The associated order ID, if any
    /// </summary>
    /// <example>550e8400-e29b-41d4-a716-446655440000</example>
    public Guid? OrderId { get; set; }
}
