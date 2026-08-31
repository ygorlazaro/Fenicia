using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Basic.Domains.DataSource.DTOs;

public record GetAllProductCategoryForDataSourceResponse([Required] Guid Id, [Required][MaxLength(200)] string Name);
