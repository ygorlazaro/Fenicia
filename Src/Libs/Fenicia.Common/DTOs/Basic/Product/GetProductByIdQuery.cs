using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Product;

public record GetProductByIdQuery([Required] Guid Id);