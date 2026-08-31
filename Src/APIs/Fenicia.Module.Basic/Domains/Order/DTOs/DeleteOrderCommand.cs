using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Basic.Domains.Order.DTOs;

public record DeleteOrderCommand([Required] Guid Id);
