namespace Fenicia.Common.Database.Requests;

using System.ComponentModel.DataAnnotations;

public class CompanyUpdateRequest
{
    [MaxLength(length: 50)]
    [Required]
    public string Name { get; set; } = null!;

    [Required]
    public string Timezone { get; set; } = null!;
}
