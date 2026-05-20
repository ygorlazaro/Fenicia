using MediatR;

namespace Fenicia.Module.Basic.Domains.Employee.Commands;

/// <summary>
///     Command record for deleting (soft delete) an employee.
/// </summary>
public record DeleteEmployeeCommand(Guid Id) : IRequest;
