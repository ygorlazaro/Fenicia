using System.ComponentModel.DataAnnotations;
using Fenicia.Common;

namespace Fenicia.Common.DTOs.Basic.Position;

public record GetAllPositionResponse([Required] Guid Id, [Required] [MaxLength(200)] string Name) : ICrudItem;