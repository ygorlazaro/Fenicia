using Fenicia.Common.Enums.Auth;

namespace Fenicia.Auth.Domains.Subscription.DTOs.Responses;

public record UserModuleResponse(Guid Id, string Name, ModuleType Type);
