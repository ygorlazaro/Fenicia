using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Enums.Auth;

namespace Fenicia.Common.Data.Responses.Auth;

[Serializable]
public class SubscriptionResponse
{
    public SubscriptionResponse()
    {
        
    }

    public SubscriptionResponse(SubscriptionModel model)
    {
        this.Id = model.Id;
        this.Status = model.Status;
        this.StartDate = model.StartDate;
        this.EndDate = model.EndDate;
        this.OrderId = model.OrderId;
    }

    public Guid Id { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public SubscriptionStatus Status { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    public Guid? OrderId { get; set; }
}