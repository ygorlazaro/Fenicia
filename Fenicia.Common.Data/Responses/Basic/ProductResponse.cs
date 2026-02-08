using Fenicia.Common.Data.Models.Basic;

namespace Fenicia.Common.Data.Responses.Basic;

public class ProductResponse(ProductModel model)
{
    public Guid Id { get; set; } = model.Id;

    public string Name { get; set; } = model.Name;

    public decimal? CostPrice { get; set; } = model.CostPrice;

    public decimal SellingPrice { get; set; } = model.SalesPrice;

    public double Quantity { get; set; } = model.Quantity;

    public Guid CategoryId { get; set; } = model.CategoryId;
}