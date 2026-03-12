namespace Fenicia.Module.Basic.Domains.Supplier.Responses;

public record UpdateSupplierResponse(
    Guid Id,
    string? Cnpj);