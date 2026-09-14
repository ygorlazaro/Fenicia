using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Order;

public record GetOrderByIdQuery([Required] Guid Id);