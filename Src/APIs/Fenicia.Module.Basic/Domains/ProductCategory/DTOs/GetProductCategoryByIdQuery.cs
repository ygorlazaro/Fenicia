using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Basic.Domains.ProductCategory.DTOs;

public record GetProductCategoryByIdQuery([Required] Guid Id);