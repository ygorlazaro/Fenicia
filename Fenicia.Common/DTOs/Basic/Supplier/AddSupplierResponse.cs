using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Supplier;

public class AddSupplierResponse()
{
    public AddSupplierResponse(Guid id, string? cnpj)
        : this()
    {
        Id = id;
        Cnpj = cnpj;
    }

    [Required]
    public Guid Id { get; init; }

    [MaxLength(14)]
    public string? Cnpj { get; set; }
}
