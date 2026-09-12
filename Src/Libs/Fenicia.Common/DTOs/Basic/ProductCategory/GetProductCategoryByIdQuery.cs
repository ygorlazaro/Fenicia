using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.ProductCategory;

public record GetProductCategoryByIdQuery([Required] Guid Id);