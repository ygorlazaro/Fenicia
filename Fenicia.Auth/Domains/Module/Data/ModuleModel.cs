namespace Fenicia.Auth.Domains.Module.Data;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

using Common.Database;
using Common.Enums;

using Microsoft.EntityFrameworkCore;

using OrderDetail.Data;

using SubscriptionCredit.Data;

/// <summary>
///     Represents a module entity in the system.
/// </summary>
[Table(name: "modules")]
public class ModuleModel : BaseModel
{
    /// <summary>
    ///     Gets or sets the name of the module.
    /// </summary>
    [Required(ErrorMessage = "Module name is required")]
    [MaxLength(length: 30, ErrorMessage = "Module name cannot exceed 30 characters")]
    [MinLength(length: 3, ErrorMessage = "Module name must be at least 3 characters")]
    [Column(name: "name")]
    public string Name { get; set; } = null!;

    /// <summary>
    ///     Gets or sets the amount associated with the module.
    /// </summary>
    [Required(ErrorMessage = "Amount is required")]
    [Range(minimum: 0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    [Column(name: "amount")]
    [Precision(precision: 18, scale: 2)]
    public decimal Amount { get; set; }

    /// <summary>
    ///     Gets or sets the type of the module.
    /// </summary>
    [Required(ErrorMessage = "Module type is required")]
    [Column(name: "type")]
    [EnumDataType(typeof(ModuleType))]
    public ModuleType Type { get; set; }

    /// <summary>
    ///     Gets or sets the list of subscription credits associated with this module.
    /// </summary>
    [JsonIgnore]
    [NotNull]
    public virtual List<SubscriptionCreditModel> SubscriptionCredits { get; set; } = [];

    /// <summary>
    ///     Gets or sets the list of order details associated with this module.
    /// </summary>
    [JsonIgnore]
    [NotNull]
    public virtual List<OrderDetailModel> OrderDetails { get; set; } = [];
}
