namespace Fenicia.Module.Basic.Domains.Employee.Responses;

public record UpdateEmployeeResponse(
    Guid Id,
    Guid PositionId,
Guid PersonId);