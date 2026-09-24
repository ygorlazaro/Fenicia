using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Position;

/// <summary>
/// Response DTO containing position data.
/// </summary>
/// <param name="id">The unique identifier of the position.</param>
/// <param name="name">The position name.</param>
public record PositionResponse(
    [Required] Guid Id,
    [Required] [MaxLength(50)] string Name);