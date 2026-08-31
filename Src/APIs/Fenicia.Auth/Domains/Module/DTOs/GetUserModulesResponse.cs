using System.ComponentModel.DataAnnotations;
using Fenicia.Common.Enums.Auth;

namespace Fenicia.Auth.Domains.Module.DTOs;

public record GetUserModulesResponse([Required] Guid Id, [Required][MaxLength(200)] string Name, [Required] ModuleType Type);
