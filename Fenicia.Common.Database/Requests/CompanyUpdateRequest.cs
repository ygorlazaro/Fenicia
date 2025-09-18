namespace Fenicia.Common.Database.Requests;

using System.ComponentModel.DataAnnotations;

public class CompanyUpdateRequest
{
    [MaxLength(length: 50, ErrorMessage = "The field {0} must be a string with a maximum length of {1}.")]
    [Required]
    public string Name { get; set; } = null!;

    [Required]
    public string Timezone { get; set; } = null!;
}
