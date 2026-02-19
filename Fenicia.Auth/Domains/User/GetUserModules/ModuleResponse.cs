using Fenicia.Common.Enums.Auth;

namespace Fenicia.Auth.Domains.User.GetUserModules;

public record ModuleResponse(Guid Id, string Name, ModuleType Type);