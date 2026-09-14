using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Employee;

public record DeleteEmployeeCommand([Required] Guid Id);