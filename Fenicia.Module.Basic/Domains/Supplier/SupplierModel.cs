namespace Fenicia.Module.Basic.Domains.Supplier;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Address;

using Common.Database;

[Table(name: "suppliers")]
public class SupplierModel : BaseModel
{
    [Required]
    [MaxLength(length: 50)]
    public string Name { get; set; } = null!;

    [MaxLength(length: 14)]
    public string? Cpf { get; set; } = null!;

    [Required]
    public Guid AddressId
    {
        get; set;
    }

    [ForeignKey(name: "AddressId")]
    public virtual AddressModel Address { get; set; } = null!;
}
