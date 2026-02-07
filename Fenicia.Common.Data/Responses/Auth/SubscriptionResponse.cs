using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

using Fenicia.Common.Enums.Auth;

namespace Fenicia.Common.Data.Responses.Auth;

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
}