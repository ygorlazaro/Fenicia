namespace Fenicia.Module.Basic.Domains.Product.Responses;

public record ProfitMarginResponse(
    Guid ProductId,
    string ProductName,
    string CategoryName,
    decimal CostPrice,
    decimal SalesPrice,
    decimal ProfitMargin,
    string MarginClassification);