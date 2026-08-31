using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Basic.Domains.State.DTOs;

public record GetAllStateResponse(
    [Required] Guid Id,
    [Required][MaxLength(200)] string Name,
    [Required][MaxLength(200)] string Uf);
