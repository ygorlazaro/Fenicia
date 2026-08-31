using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Basic.Domains.Product.DTOs;

public record DeleteProductCommand(

    [Required] Guid Id);
