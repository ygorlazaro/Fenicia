namespace Fenicia.Module.Basic.Domains.Employee.Add;

public record AddEmployeeResponse(
    Guid Id,
    Guid PositionId,
    Guid PersonId);