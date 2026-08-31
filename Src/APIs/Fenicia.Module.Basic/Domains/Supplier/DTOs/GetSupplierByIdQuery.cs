using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Basic.Domains.Supplier.DTOs;

public record GetSupplierByIdQuery(

    [Required] Guid Id);
