using System.ComponentModel.DataAnnotations;
using Fenicia.Common.Enums.Auth;

namespace Fenicia.Auth.Domains.Subscription.DTOs;

public record UserModuleResponse(
    [Required] Guid Id,
    [Required] [MaxLength(200)] string Name,
    [Required] ModuleType Type);