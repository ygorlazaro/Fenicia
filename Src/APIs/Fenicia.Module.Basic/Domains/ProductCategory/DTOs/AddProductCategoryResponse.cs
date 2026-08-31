using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Basic.Domains.ProductCategory.DTOs;

public record AddProductCategoryResponse(

    [Required] Guid Id,

    [Required][MaxLength(200)] string Name);
