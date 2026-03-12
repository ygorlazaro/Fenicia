namespace Fenicia.Module.Basic.Domains.Employee.Responses;

public record GetEmployeesByPositionIdResponse(Guid Id, Guid PositionId, Guid PersonId);
