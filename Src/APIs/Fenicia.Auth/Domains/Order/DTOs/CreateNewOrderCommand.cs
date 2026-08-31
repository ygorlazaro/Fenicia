using System.ComponentModel.DataAnnotations;

namespace Fenicia.Auth.Domains.Order.DTOs;

public sealed record CreateNewOrderCommand([Required] Guid UserId, [Required] Guid CompanyId, List<Guid> Modules);
