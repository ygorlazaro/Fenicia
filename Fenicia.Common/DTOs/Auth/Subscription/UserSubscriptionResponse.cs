using System.ComponentModel.DataAnnotations;
using Fenicia.Common.DTOs.Auth.Module;
using Fenicia.Common.Enums.Auth;

namespace Fenicia.Common.DTOs.Auth.Subscription;

public class UserSubscriptionResponse()
{
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

    [Required]
    public Guid Id { get; set; }

    [Required]
    public Guid CompanyId { get; set; }

    [Required]
    [MaxLength(200)]
    public string CompanyName { get; set; } = string.Empty;

    [Required]
    public EnumSubscriptionStatus Status { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    [Required]
    public IEnumerable<ModuleResponse> Modules { get; set; } = [];
}
