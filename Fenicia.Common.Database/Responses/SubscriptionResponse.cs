namespace Fenicia.Common.Database.Responses;

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

using Enums;

using Models.Auth;

[Serializable]
public class SubscriptionResponse
{
    public Guid Id
    {
        get; set;
    }

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

    public static SubscriptionResponse Convert(SubscriptionModel subscription)
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

    public override bool Equals(object? obj)
    {
        if (obj is not SubscriptionResponse other)
        {
            return false;
        }

        return Id == other.Id &&
               Status == other.Status &&
               StartDate == other.StartDate &&
               EndDate == other.EndDate &&
               OrderId == other.OrderId;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Status, StartDate, EndDate, OrderId);
    }
}
