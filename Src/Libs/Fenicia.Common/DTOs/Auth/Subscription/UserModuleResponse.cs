using System.ComponentModel.DataAnnotations;
using Fenicia.Common.Enums.Auth;

namespace Fenicia.Common.DTOs.Auth.Subscription;

public record UserModuleResponse(
    [Required] Guid Id,
    [Required] [MaxLength(200)] string Name,
    [Required] ModuleType Type);