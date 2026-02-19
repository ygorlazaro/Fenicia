using Fenicia.Common.Enums.Auth;

namespace Fenicia.Auth.Domains.Module.GetModules;

public sealed record ModuleResponse (Guid Id, string Name, ModuleType Type);