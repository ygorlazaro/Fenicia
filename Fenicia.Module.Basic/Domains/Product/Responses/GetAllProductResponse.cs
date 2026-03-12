namespace Fenicia.Module.Basic.Domains.Product.Responses;

public record GetAllProductResponse(
    Guid Id,
    string Name,
    decimal? CostPrice,
    decimal SalesPrice,
    double Quantity,
    Guid CategoryId,
    string CategoryName,
    Guid? SupplierId,
    string? SupplierName);
