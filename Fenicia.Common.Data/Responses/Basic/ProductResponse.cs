namespace Fenicia.Common.Data.Responses.Basic;

public class ProductResponse
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public decimal? CostPrice { get; set; }

    public decimal SellingPrice { get; set; }

    public double Quantity { get; set; }

    public Guid CategoryId { get; set; }
}