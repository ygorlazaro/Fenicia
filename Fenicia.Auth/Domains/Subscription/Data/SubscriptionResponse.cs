using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

using Fenicia.Auth.Enums;

namespace Fenicia.Auth.Domains.Subscription.Data;

/// <summary>
/// Response model containing subscription information for API responses
/// </summary>
/// <remarks>
/// This class represents the subscription data that is returned to API clients
/// </remarks>
[Serializable]
public class SubscriptionResponse
{
    /// </summary>
    /// <example>550e8400-e29b-41d4-a716-446655440000</example>
    /// <summary>
    /// Gets or sets the unique identifier of the subscription
    /// </summary>
    /// <value>The subscription identifier</value>
    /// <example>550e8400-e29b-41d4-a716-446655440000</example>
    [Required]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the current status of the subscription
    /// </summary>
    /// <value>The subscription status</value>
    [Required]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public SubscriptionStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the start date of the subscription
    /// </summary>
    /// <value>The date when the subscription begins</value>
    /// <example>2025-06-03T00:00:00Z</example>
    [Required]
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Gets or sets the end date of the subscription
    /// </summary>
    /// <value>The date when the subscription expires</value>
    /// <example>2026-06-03T00:00:00Z</example>
    [Required]
    public DateTime EndDate { get; set; }

    /// <summary>
    /// Gets or sets the associated order ID for the subscription
    /// </summary>
    /// <value>The order identifier, if available</value>
    /// <example>550e8400-e29b-41d4-a716-446655440000</example>
    public Guid? OrderId { get; set; }
}
