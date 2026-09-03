using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Basic.Domains.Employee.DTOs;

public record GetEmployeeByIdQuery([Required] Guid Id);