using System.ComponentModel.DataAnnotations;
using Fenicia.Common;

namespace Fenicia.Common.DTOs.Basic.ProductCategory;

public record GetAllProductCategoryResponse([Required] Guid Id,
    [Required] [MaxLength(200)] string Name) : ICrudItem;