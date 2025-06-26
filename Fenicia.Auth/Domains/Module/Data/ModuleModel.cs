using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using System.Diagnostics.CodeAnalysis;

using Fenicia.Auth.Domains.OrderDetail.Data;
using Fenicia.Auth.Domains.SubscriptionCredit.Data;
using Fenicia.Common.Database;
using Fenicia.Common.Enums;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Module.Data;

/// <summary>
/// Represents a module entity in the system.
/// </summary>
[Table("modules")]
public class ModuleModel : BaseModel
{
    /// <summary>
    /// Gets or sets the name of the module.
    /// </summary>
    [Required(ErrorMessage = "Module name is required")]
    [MaxLength(30, ErrorMessage = "Module name cannot exceed 30 characters")]
    [MinLength(3, ErrorMessage = "Module name must be at least 3 characters")]
    [Column("name")]
    public string Name { get; set; } = null!;

    /// <summary>
    /// Gets or sets the amount associated with the module.
    /// </summary>
    [Required(ErrorMessage = "Amount is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    [Column("amount")]
    [Precision(18, 2)]
    public decimal Amount { get; set; }

    /// <summary>
    /// Gets or sets the type of the module.
    /// </summary>
    [Required(ErrorMessage = "Module type is required")]
    [Column("type")]
    [EnumDataType(typeof(ModuleType))]
    public ModuleType Type { get; set; }

    /// <summary>
    /// Gets or sets the list of subscription credits associated with this module.
    /// </summary>
    [JsonIgnore]
    [NotNull]
    public virtual List<SubscriptionCreditModel> SubscriptionCredits { get; set; } = [];

    /// <summary>
    /// Gets or sets the list of order details associated with this module.
    /// </summary>
    [JsonIgnore]
    [NotNull]
    public virtual List<OrderDetailModel> OrderDetails { get; set; } = [];
}
