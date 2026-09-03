using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fenicia.Common.Data.Models.Basic;

[Table("people", Schema = "basic")]
public sealed class PersonModel : BaseCompanyModel
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

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
    public DateTime? DateOfBirth { get; init; }

    [Column("photo_url")]
    [MaxLength(500)]
    public string? PhotoUrl { get; init; }

    [MaxLength(1000)]
    public string? Notes { get; init; }

    public ICollection<PersonAddressModel> PersonAddresses { get; init; } = [];

    public CustomerModel? Customer { get; init; }

    public EmployeeModel? Employee { get; init; }
}