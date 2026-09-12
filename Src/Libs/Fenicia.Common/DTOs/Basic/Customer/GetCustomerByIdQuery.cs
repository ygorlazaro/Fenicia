using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Customer;

public record GetCustomerByIdQuery([Required] Guid Id);