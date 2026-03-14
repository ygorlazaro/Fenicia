using Fenicia.Common.Enums.Auth;

namespace Fenicia.Auth.Domains.Module.Responses;

/// <summary>
///     Response DTO for user-specific module access information.
/// </summary>
public record GetUserModulesResponse(Guid Id, string Name, ModuleType Type);