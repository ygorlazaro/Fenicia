using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Fenicia.Common.Database;

namespace Fenicia.Module.Basic.Contexts.Models;

[Table("customers")]
public class CustomerModel : BaseModel
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = null!;

    [MaxLength(14)]
    public string? Cpf { get; set; } = null!;

    [Required]
    public Guid AddressId { get; set; }

    [ForeignKey("AddressId")]
    public virtual AddressModel Address { get; set; } = null!;
}