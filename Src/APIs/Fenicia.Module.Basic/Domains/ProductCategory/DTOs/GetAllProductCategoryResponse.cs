using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Basic.Domains.ProductCategory.DTOs;

public record GetAllProductCategoryResponse(

    [Required] Guid Id,

    [Required][MaxLength(200)] string Name);
