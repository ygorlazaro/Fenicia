using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Position;

public record AddPositionCommand([Required] [MaxLength(200)] string Name);