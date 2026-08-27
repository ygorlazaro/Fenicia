using Fenicia.Module.Basic.Domains.Position.DTOs.Responses;


namespace Fenicia.Module.Basic.Domains.Position.DTOs.Commands;

public record UpdatePositionCommand(Guid Id, string Name);
