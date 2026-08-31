using System.ComponentModel.DataAnnotations;
using Fenicia.Common.Enums.Auth;
using Fenicia.Common.Enums.Basic;

namespace Fenicia.Module.Basic.Domains.Order.DTOs;

public record CreateOrderCommand(
    [Required] Guid UserId,
    [Required] Guid CustomerId,
    [Required] DateTime SaleDate,
    [Required] OrderStatus Status,
    List<OrderDetailCommand> Details,
    [Required] PaymentMethod PaymentMethod,
    Guid? EmployeeId = null,
    string? Notes = null,
    decimal DiscountAmount = 0);
