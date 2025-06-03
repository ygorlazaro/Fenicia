using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Fenicia.Common.Database;

namespace Fenicia.Module.Basic.Contexts.Models;

[Table("addresses")]
public class AddressModel : BaseModel
{
    [Required]
    [MaxLength(100)]
    public string Street { get; set; } = null!;

    [Required]
    [MaxLength(10)]
    public string Number { get; set; } = null!;

    [MaxLength(10)]
    public string Complement { get; set; } = null!;

    [Required]
    [MaxLength(9)]
    public string ZipCode { get; set; } = null!;

    [Required]
    public Guid StateId { get; set; }

    [Required]
    [MaxLength(30)]
    public string City { get; set; } = null!;

    [ForeignKey("StateId")]
    public virtual StateModel State { get; set; } = null!;

    public virtual List<CustomerModel> Customers { get; set; } = null!;
}