using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.DataSource;

public record GetAllPositionForDataSourceResponse([Required] Guid Id, [Required] [MaxLength(200)] string Name);