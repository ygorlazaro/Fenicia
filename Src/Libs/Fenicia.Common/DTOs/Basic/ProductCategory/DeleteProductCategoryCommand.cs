using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.ProductCategory;

public record DeleteProductCategoryCommand([Required] Guid Id);