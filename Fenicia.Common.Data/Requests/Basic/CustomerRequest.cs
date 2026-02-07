namespace Fenicia.Common.Data.Requests.Basic;

public class CustomerRequest
{
    public Guid Id { get; set; }

    public PersonRequest Person { get; set; }
}