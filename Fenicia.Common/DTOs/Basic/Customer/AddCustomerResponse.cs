using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Customer;

public record AddCustomerResponse([Required] Guid Id, [Required] Guid PersonId);