namespace Fenicia.Module.Basic.Domains.Employee.GetById;

public record GetEmployeeByIdResponse(
    Guid Id,
    Guid PositionId,
    Guid PersonId);
