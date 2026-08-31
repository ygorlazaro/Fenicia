using System.ComponentModel.DataAnnotations;

namespace Fenicia.Auth.Domains.Order.DTOs;

public record CreateNewOrderResponse([Required] Guid OrderId);
