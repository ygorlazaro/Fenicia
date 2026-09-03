using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Basic.Domains.Inventory.DTOs;

public record GetInventoryByProductQuery([Required] Guid ProductId, int Page = 1, int PerPage = 10);