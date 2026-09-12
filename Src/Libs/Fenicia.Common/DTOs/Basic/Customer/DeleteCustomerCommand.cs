using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Customer;

public record DeleteCustomerCommand([Required] Guid Id);