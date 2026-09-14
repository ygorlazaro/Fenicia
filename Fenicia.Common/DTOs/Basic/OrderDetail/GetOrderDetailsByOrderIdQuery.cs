using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.OrderDetail;

public record GetOrderDetailsByOrderIdQuery([Required] Guid OrderId);