using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Employee;

public record GetEmployeeByIdQuery([Required] Guid Id);