namespace Fenicia.Module.Basic.Domains.Employee.GetAll;

public record GetAllEmployeeResponse(
    Guid Id,
    Guid PositionId,
    Guid PersonId);
