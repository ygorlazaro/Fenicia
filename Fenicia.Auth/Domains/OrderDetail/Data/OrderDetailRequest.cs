using System.ComponentModel.DataAnnotations;

namespace Fenicia.Auth.Domains.OrderDetail.Data;

/// <summary>
/// Represents a request model for a single module order detail in the system.
/// This class encapsulates the necessary information required to process a module order.
/// </summary>
/// <remarks>
/// This model is used primarily in API endpoints and service layer operations
/// related to module ordering functionality.
/// </remarks>
public class OrderDetailRequest
{
    /// <summary>
    /// Gets or sets the unique identifier of the module being ordered.
    /// </summary>
    /// <value>
    /// A globally unique identifier (GUID) that uniquely identifies the module in the system.
    /// </value>
    /// <remarks>
    /// This identifier must correspond to an existing module in the system.
    /// The GUID should be in the standard 32-digit hexadecimal format.
    /// </remarks>
    /// <example>550e8400-e29b-41d4-a716-446655440000</example>
    [Required(ErrorMessage = "Module ID is required")]
    [Display(Name = "Module ID")]
    public Guid ModuleId { get; set; }
}
