using System.ComponentModel.DataAnnotations;
using Fenicia.Common.Enums.Auth;

namespace Fenicia.Auth.Domains.Module.DTOs;

public sealed record GetModuleResponse(
    [Required] Guid Id,
    [Required] [MaxLength(200)] string Name,
    [Required] ModuleType Type,
    [MaxLength(200)] string? Description,
    [MaxLength(200)] string? Icon,
    bool IsActive,
    int SortOrder,
    decimal? Price);