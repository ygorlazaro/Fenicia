using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fenicia.Common.Data.Models.Basic;

[Table("people", Schema = "basic")]
public class PersonModel : BaseCompanyModel
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = null!;

    [MaxLength(14)]
    public string? Document
    {
        get;
        set => field = value != null ? new string([.. value.Where(char.IsDigit)]) : null;
    }

    [MaxLength(50)]
    [EmailAddress]
    public string? Email { get; set; }

    [MaxLength(20)]
    public string? PhoneNumber
    {
        get;
        set => field = value != null ? new string([.. value.Where(char.IsDigit)]) : null;
    }

    [DataType(DataType.Date)]
    public DateTime? DateOfBirth { get; set; }

    [Column("photo_url")]
    [MaxLength(500)]
    public string? PhotoUrl { get; set; }

    [MaxLength(1000)]
    public string? Notes { get; set; }

    public virtual ICollection<PersonAddressModel> PersonAddresses { get; set; } = [];

    public virtual CustomerModel? Customer { get; set; }

    public virtual EmployeeModel? Employee { get; set; }
}
