using Fenicia.Common.Enums.Auth;

namespace Fenicia.Auth.Domains.Module;

public record GetUserModulesResponse(Guid Id, string Name, ModuleType Type);