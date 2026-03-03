using Fenicia.Common.Enums.Auth;

namespace Fenicia.Auth.Domains.User.GetUserModules;

public record GetUserModulesResponse(Guid Id, string Name, ModuleType Type);