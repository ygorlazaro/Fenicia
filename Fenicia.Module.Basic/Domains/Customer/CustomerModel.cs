namespace Fenicia.Module.Basic.Domains.Customer;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Address;

using Common.Database;

[Table(name: "customers")]
public class CustomerModel : BaseModel
{
    [Required]
    [MaxLength(length: 50)]
    public string Name { get; set; } = null!;

    [MaxLength(length: 14)]
    public string? Cpf { get; set; } = null!;

    [Required]
    public Guid AddressId { get; set; }

    [ForeignKey(name: "AddressId")]
    public virtual AddressModel Address { get; set; } = null!;
}
