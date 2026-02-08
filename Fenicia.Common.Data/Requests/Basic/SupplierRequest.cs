namespace Fenicia.Common.Data.Requests.Basic;

public class SupplierRequest
{
    public Guid Id { get; set; }

    public PersonRequest Person { get; set; } = null!;

    public string Cnpj { get; set; } = string.Empty;
}