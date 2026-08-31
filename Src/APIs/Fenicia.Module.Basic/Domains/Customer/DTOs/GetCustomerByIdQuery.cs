using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Basic.Domains.Customer.DTOs;

public record GetCustomerByIdQuery([Required] Guid Id);
