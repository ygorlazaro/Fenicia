using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Customer;

public record UpdateCustomerResponse([Required] Guid Id, [Required] Guid PersonId);