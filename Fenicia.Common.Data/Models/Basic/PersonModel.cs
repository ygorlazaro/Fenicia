using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fenicia.Common.Data.Models.Basic;

[Table("people")]
public class PersonModel : BaseModel
{
    [Required]
    [MaxLength(50)]
    public string Name
    {
        get;
        set;
    }

    [MaxLength(14)]
    public string? Cpf
    {
        get;
        set;
    }

        = null!;

    [Required]
    [MaxLength(100)]
    public string Street
    {
        get;
        set;
    }

        = null!;

    [Required]
    [MaxLength(10)]
    public string Number
    {
        get;
        set;
    }

        = null!;

    [MaxLength(10)]
    public string Complement
    {
        get;
        set;
    }

        = null!;

    [MaxLength(50)]
    public string Neighborhood
    {
        get;
        set;
    }

        = null!;

    [Required]
    [MaxLength(8)]
    public string ZipCode
    {
        get;
        set;
    }

        = null!;

    [Required]
    public Guid StateId
    {
        get;
        set;
    }

    [Required]
    [MaxLength(50)]
    public string City
    {
        get;
        set;
    }

        = null!;

    [MaxLength(20)]
    public string PhoneNumber
    {
        get;
        set;
    }

    [ForeignKey(nameof(StateId))]
    public virtual StateModel State
    {
        get;
        set;
    }

        = null!;

    public virtual CustomerModel? Customer
    {
        get;
        set;
    }

    public virtual EmployeeModel? Employee
    {
        get;
        set;
    }
}