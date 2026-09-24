using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.Order;

/// <summary>
/// Request DTO for creating an order.
/// </summary>
public class OrderRequest()
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OrderRequest"/> class with the specified order details.
    /// </summary>
    /// <param name="userId">The unique identifier of the user placing the order.</param>
    /// <param name="companyId">The unique identifier of the company.</param>
    /// <param name="modules">The list of module identifiers to include in the order.</param>
    public OrderRequest(Guid userId, Guid companyId, List<Guid> modules)
        : this()
    {
        UserId = userId;
        CompanyId = companyId;
        Modules = modules;
    }

    /// <summary>
    /// Gets or sets the unique identifier of the user placing the order.
    /// </summary>
    [Required]
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the company.
    /// </summary>
    [Required]
    public Guid CompanyId { get; set; }

    /// <summary>
    /// Gets or sets the list of module identifiers to include in the order.
    /// </summary>
    [Required]
    public List<Guid> Modules { get; set; } = [];
}
