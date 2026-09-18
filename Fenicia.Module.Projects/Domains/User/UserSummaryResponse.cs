using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Projects.Domains.User;

public class UserSummaryResponse()
{
    public UserSummaryResponse(
        [Required] Guid id,
        [Required] [MaxLength(48)] string name,
        [Required] [MaxLength(48)] string email)
        : this()
    {
        Id = id;
        Name = name;
        Email = email;
    }

    [Required]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(48)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(48)]
    public string Email { get; set; } = string.Empty;
}
