using Fenicia.Common.Enums.Auth;

namespace Fenicia.Auth.Domains.Module.Responses;

/// <summary>
/// Response DTO for module information in public module listings.
/// </summary>
public sealed record GetModuleResponse(Guid Id, string Name, ModuleType Type);