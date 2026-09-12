using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Product;

public record GetProductsByCategoryIdQuery([Required] Guid CategoryId, int Page = 1, int PerPage = 10);