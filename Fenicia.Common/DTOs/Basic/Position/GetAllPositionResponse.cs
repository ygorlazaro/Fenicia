using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Position;

public record GetAllPositionResponse([Required] Guid Id, [Required][MaxLength(200)] string Name) : ICrudItem;