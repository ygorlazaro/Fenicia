using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fenicia.Common.Data.Models;

[Table("people", Schema = "basic")]
public class BasicPersonModel : BaseCompanyModel
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = null!;

    [MaxLength(14)]
    public string? Document
    {
        get;
        set => field = value != null ? new string(value.Where(char.IsDigit).ToArray()) : null;
    }

    [MaxLength(100)]
    public string? Street { get; set; }

    [MaxLength(20)]
    public string? Number
    {
        get;
        set => field = value != null ? new string(value.Where(char.IsDigit).ToArray()) : null;
    }

    [MaxLength(20)]
    public string? Complement { get; set; }

    [MaxLength(50)]
    public string? Neighborhood { get; set; }

    [MaxLength(8)]
    public string? ZipCode
    {
        get;
        set => field = value != null ? new string(value.Where(char.IsDigit).ToArray()) : null;
    }

    public Guid? StateId { get; set; }

    [MaxLength(50)]
    public string? City { get; set; }
    
    [MaxLength(50)]
    [EmailAddress]
    public string? Email { get; set; }

    [MaxLength(20)]
    public string? PhoneNumber
    {
        get;
        set => field = value != null ? new string(value.Where(char.IsDigit).ToArray()) : null;
    }

    [ForeignKey(nameof(StateId))]
    public virtual AuthStateModel StateModel { get; set; } = null!;

    public virtual BasicCustomerModel? Customer { get; set; }

    public virtual BasicEmployeeModel? Employee { get; set; }
}
