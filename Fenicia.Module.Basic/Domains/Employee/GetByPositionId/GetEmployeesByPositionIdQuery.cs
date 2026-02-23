namespace Fenicia.Module.Basic.Domains.Employee.GetByPositionId;

public record GetEmployeesByPositionIdQuery(Guid PositionId, int Page = 1, int PerPage = 10);
