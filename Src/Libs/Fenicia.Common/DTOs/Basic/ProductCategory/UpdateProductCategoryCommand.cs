using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.ProductCategory;

public record UpdateProductCategoryCommand(
    [Required] Guid Id,
    [Required] [MaxLength(200)] string Name);