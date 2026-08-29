using Fenicia.Common.Enums.Auth;

namespace Fenicia.Auth.Domains.Module.DTOs;

public sealed record GetModuleResponse(
    Guid Id,
    string Name,
    ModuleType Type,
    string? Description,
    string? Icon,
    bool IsActive,
    int SortOrder,
    decimal Price);
