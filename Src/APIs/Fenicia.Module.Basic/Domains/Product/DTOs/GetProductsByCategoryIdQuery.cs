using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Basic.Domains.Product.DTOs;

public record GetProductsByCategoryIdQuery([Required] Guid CategoryId, int Page = 1, int PerPage = 10);
