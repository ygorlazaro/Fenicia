namespace Fenicia.Module.Basic.Domains.Supplier.GetById;

public record GetSupplierByIdResponse(
    Guid Id,
    string? Cnpj);