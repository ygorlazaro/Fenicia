using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Inventory;

public record GetInventoryByCategoryQuery([Required] Guid CategoryId, int Page = 1, int PerPage = 10);