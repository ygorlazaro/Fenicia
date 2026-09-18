using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Supplier;

public class GetSupplierByIdQuery
{
    public GetSupplierByIdQuery()
    {
    }

    public GetSupplierByIdQuery(Guid id)
    {
        Id = id;
    }

    [Required]
    public Guid Id { get; set; }
}
