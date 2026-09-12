using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Position;

public record AddPositionResponse([Required] Guid Id, [Required] [MaxLength(200)] string Name);