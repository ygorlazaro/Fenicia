namespace Fenicia.Auth.Domains.Submodule.GetByModuleId;

public record GetByModuleResponse(Guid Id, string Name, string? Description, Guid ModuleId, string Route);