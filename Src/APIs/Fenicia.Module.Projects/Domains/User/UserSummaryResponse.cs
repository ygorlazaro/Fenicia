using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Projects.Domains.User;

public record UserSummaryResponse(
    [Required] Guid Id,
    [Required] [MaxLength(48)] string Name,
    [Required] [MaxLength(48)] string Email);
