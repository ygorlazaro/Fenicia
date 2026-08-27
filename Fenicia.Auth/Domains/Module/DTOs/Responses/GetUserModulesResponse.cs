using Fenicia.Common.Enums.Auth;

namespace Fenicia.Auth.Domains.Module.DTOs.Responses;

public record GetUserModulesResponse(Guid Id, string Name, ModuleType Type);