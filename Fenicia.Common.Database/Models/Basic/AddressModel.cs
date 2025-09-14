namespace Fenicia.Common.Database.Models.Basic;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Database;

[Table("addresses")]
public class AddressModel : BaseModel
{
    [Required]
    [MaxLength(length: 100)]
    public string Street { get; set; } = null!;

    [Required]
    [MaxLength(length: 10)]
    public string Number { get; set; } = null!;

    [MaxLength(length: 10)]
    public string Complement { get; set; } = null!;

    [Required]
    [MaxLength(length: 9)]
    public string ZipCode { get; set; } = null!;

    [Required]
    public Guid StateId
    {
        get; set;
    }

    [Required]
    [MaxLength(length: 30)]
    public string City { get; set; } = null!;

    [ForeignKey("StateId")]
    public virtual StateModel State { get; set; } = null!;

    public virtual List<CustomerModel> Customers { get; set; } = null!;
}
