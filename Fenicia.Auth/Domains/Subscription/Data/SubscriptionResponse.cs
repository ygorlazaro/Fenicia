namespace Fenicia.Auth.Domains.Subscription.Data;

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

using Enums;

[Serializable]
public class SubscriptionResponse
{
    [Required]
    public Guid Id
    {
        get; set;
    }

    [Required]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public SubscriptionStatus Status
    {
        get; set;
    }

    [Required]
    public DateTime StartDate
    {
        get; set;
    }

    [Required]
    public DateTime EndDate
    {
        get; set;
    }

    public Guid? OrderId
    {
        get; set;
    }

    public static SubscriptionResponse? Convert(SubscriptionModel subscription)
    {
        return new SubscriptionResponse
        {
            Id = subscription.Id,
            Status = subscription.Status,
            StartDate = subscription.StartDate,
            EndDate = subscription.EndDate,
            OrderId = subscription.OrderId
        };
    }
}
