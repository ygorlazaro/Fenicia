using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Basic.Domains.Inventory.DTOs;

public record GetInventoryByCategoryQuery([Required] Guid CategoryId, int Page = 1, int PerPage = 10);
