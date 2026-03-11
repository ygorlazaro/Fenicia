using Fenicia.Common.Enums.Auth;

namespace Fenicia.Auth.Domains.Module.Responses;

public sealed record GetModuleResponse(Guid Id, string Name, ModuleType Type);