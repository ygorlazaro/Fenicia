using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.Data.Requests.Auth;

public class CompanyUpdateRequest
{
    [MaxLength(50, ErrorMessage = "The field {0} must be a string with a maximum length of {1}.")]
    [Required]
    public string Name { get; set; } = null!;

    [Required]
    public string Timezone { get; set; } = null!;
}
