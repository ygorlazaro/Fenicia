using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Order;

public record DeleteOrderCommand([Required] Guid Id);