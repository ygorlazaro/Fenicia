namespace Fenicia.Module.Basic.Domains.Employee.DTOs;

public record GetEmployeesByPositionIdQuery(Guid PositionId, int Page = 1, int PerPage = 10);
