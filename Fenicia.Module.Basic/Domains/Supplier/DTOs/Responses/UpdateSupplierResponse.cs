namespace Fenicia.Module.Basic.Domains.Supplier.DTOs.Responses;

public record UpdateSupplierResponse(

    Guid Id,

    string? Cnpj);