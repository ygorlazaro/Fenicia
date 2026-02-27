using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fenicia.Common.Data.Models;

[Table("people", Schema = "basic")]
public class BasicPerson : BaseModel
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = null!;

    [MaxLength(14)]
    public string? Document { get; set; }

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


    public Guid? StateId { get; set; }

    [MaxLength(50)]
    public string? City { get; set; }
    
    [MaxLength(50)]
    [EmailAddress]
    public string? Email { get; set; }

    [MaxLength(20)]
    public string? PhoneNumber { get; set; }
    
    [ForeignKey(nameof(StateId))]
    public virtual AuthState State { get; set; } = null!;

    public virtual BasicCustomer? Customer { get; set; }

    public virtual BasicEmployee? Employee { get; set; }
}
