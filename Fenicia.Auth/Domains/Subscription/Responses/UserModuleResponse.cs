using Fenicia.Common.Enums.Auth;

namespace Fenicia.Auth.Domains.Subscription.Responses;

public record UserModuleResponse(Guid Id, string Name, ModuleType Type);
