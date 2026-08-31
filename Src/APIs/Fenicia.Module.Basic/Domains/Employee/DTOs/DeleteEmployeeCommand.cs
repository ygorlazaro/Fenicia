using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Basic.Domains.Employee.DTOs;

public record DeleteEmployeeCommand([Required] Guid Id);
