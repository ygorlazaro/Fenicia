using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Basic.Domains.Order.DTOs;

public record SalesTrendResponse([Required][MaxLength(200)] string Period, [Required] DateTime Date, int OrderCount, decimal TotalValue, int TotalItems);
