using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.Order;

public class OrderRequest()
{
    public OrderRequest(Guid userId, Guid companyId, List<Guid> modules)
        : this()
    {
        UserId = userId;
        CompanyId = companyId;
        Modules = modules;
    }

    [Required]
    public Guid UserId { get; set; }

    [Required]
    public Guid CompanyId { get; set; }

    [Required]
    public List<Guid> Modules { get; set; } = [];
}
