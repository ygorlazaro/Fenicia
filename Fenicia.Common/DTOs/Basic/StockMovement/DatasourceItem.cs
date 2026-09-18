namespace Fenicia.Common.DTOs.Basic.StockMovement;

public class DatasourceItem()
{
    public DatasourceItem(Guid id, string name)
        : this()
    {
        Id = id;
        Name = name;
    }

    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;
}
