using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Inventory;

public record GetInventoryByProductQuery([Required] Guid ProductId, int Page = 1, int PerPage = 10);