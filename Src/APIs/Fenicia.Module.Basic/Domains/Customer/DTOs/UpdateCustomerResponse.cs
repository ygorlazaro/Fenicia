using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Basic.Domains.Customer.DTOs;

public record UpdateCustomerResponse([Required] Guid Id, [Required] Guid PersonId);
