using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.Order;

public sealed record CreateNewOrderCommand([Required] Guid UserId, [Required] Guid CompanyId, List<Guid> Modules);