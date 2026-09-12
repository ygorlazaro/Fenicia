using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.State;

public record GetAllStateResponse(
    [Required] Guid Id,
    [Required] [MaxLength(200)] string Name,
    [Required] [MaxLength(200)] string Uf);