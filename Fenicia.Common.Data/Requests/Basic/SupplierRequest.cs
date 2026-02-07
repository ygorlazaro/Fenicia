namespace Fenicia.Common.Data.Requests.Basic;

public class SupplierRequest
{
    public Guid Id { get; set; }

    public PersonRequest Person { get; set; }
}