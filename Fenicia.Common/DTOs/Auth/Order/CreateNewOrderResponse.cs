using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.Order;

public record CreateNewOrderResponse([Required] Guid OrderId);