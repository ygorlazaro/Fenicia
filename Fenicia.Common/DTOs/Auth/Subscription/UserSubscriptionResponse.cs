using System.ComponentModel.DataAnnotations;
using Fenicia.Common.DTOs.Auth.Module;
using Fenicia.Common.Enums.Auth;

namespace Fenicia.Common.DTOs.Auth.Subscription;

/// <summary>
/// Response DTO containing user subscription details.
/// </summary>
public class UserSubscriptionResponse()
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserSubscriptionResponse"/> class with the specified subscription details.
    /// </summary>
    /// <param name="id">The unique identifier of the subscription.</param>
    /// <param name="companyId">The unique identifier of the company.</param>
    /// <param name="companyName">The name of the company.</param>
    /// <param name="status">The subscription status.</param>
    /// <param name="startDate">The subscription start date.</param>
    /// <param name="endDate">The subscription end date (optional).</param>
    public UserSubscriptionResponse(
        Guid id,
        Guid companyId,
        string companyName,
        EnumSubscriptionStatus status,
        DateTime startDate,
        DateTime? endDate)
        : this()
    {
        Id = id;
        CompanyId = companyId;
        CompanyName = companyName;
        Status = status;
        StartDate = startDate;
        EndDate = endDate;
    }

    /// <summary>
    /// Gets or sets the unique identifier of the subscription.
    /// </summary>
    [Required]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the company.
    /// </summary>
    [Required]
    public Guid CompanyId { get; set; }

    /// <summary>
    /// Gets or sets the name of the company.
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string CompanyName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the subscription status.
    /// </summary>
    [Required]
    public EnumSubscriptionStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the subscription start date.
    /// </summary>
    [Required]
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Gets or sets the subscription end date.
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Gets or sets the list of modules included in the subscription.
    /// </summary>
    [Required]
    public IEnumerable<ModuleResponse> Modules { get; set; } = [];
}
