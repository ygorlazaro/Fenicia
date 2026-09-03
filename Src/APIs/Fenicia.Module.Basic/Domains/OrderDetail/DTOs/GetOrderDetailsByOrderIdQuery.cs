using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Basic.Domains.OrderDetail.DTOs;

public record GetOrderDetailsByOrderIdQuery([Required] Guid OrderId);