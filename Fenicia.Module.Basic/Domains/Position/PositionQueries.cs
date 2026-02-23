namespace Fenicia.Module.Basic.Domains.Position;

public record GetAllPositionQuery(int Page = 1, int PerPage = 10);
public record GetPositionByIdQuery(Guid Id);
public record AddPositionCommand(Guid Id, string Name);
public record UpdatePositionCommand(Guid Id, string Name);
public record DeletePositionCommand(Guid Id);

public record PositionResponse(Guid Id, string Name);
