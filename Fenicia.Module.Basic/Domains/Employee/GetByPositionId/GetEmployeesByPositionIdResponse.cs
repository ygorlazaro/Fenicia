namespace Fenicia.Module.Basic.Domains.Employee.GetByPositionId;

public record GetEmployeesByPositionIdResponse(Guid Id, Guid PositionId, Guid PersonId);