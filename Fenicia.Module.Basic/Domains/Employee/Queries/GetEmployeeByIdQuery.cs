namespace Fenicia.Module.Basic.Domains.Employee.Queries;

/// <summary>
/// Query record for retrieving a specific employee by their unique identifier.
/// </summary>
public record GetEmployeeByIdQuery(Guid Id);
