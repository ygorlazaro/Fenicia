using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fenicia.Common.Data.Models.Basic;

[Table("people")]
public class PersonModel : BaseModel
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; }

    [MaxLength(14)]
    public string? Cpf { get; set; }

    [MaxLength(100)]
    public string? Street { get; set; }


    [MaxLength(10)]
    public string? Number { get; set; }


    [MaxLength(10)]
    public string? Complement { get; set; }


    [MaxLength(50)]
    public string? Neighborhood { get; set; }

    [MaxLength(8)]
    public string? ZipCode { get; set; }


    [Required]
    public Guid StateId { get; set; }

    [MaxLength(50)]
    public string? City { get; set; }
    
    [MaxLength(50)]
    [EmailAddress]
    public string? Email { get; set; }


    [MaxLength(20)]
    public string? PhoneNumber { get; set; }
    
    [MaxLength(20)]
    public string? Document { get; set; }

    [ForeignKey(nameof(StateId))]
    public virtual StateModel State { get; set; } = null!;

    public virtual CustomerModel? Customer { get; set; }

    public virtual EmployeeModel? Employee { get; set; }
}