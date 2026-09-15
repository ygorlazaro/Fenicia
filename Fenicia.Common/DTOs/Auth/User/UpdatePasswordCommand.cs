using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.User;

public class UpdatePasswordCommand()
{
    public UpdatePasswordCommand(Guid userId, string password)
        : this()
    {
        UserId = userId;
        Password = password;
    }

    [Required]
    public Guid UserId { get; set; }

    [Required]
    [StringLength(200)]
    public string Password { get; set; } = string.Empty;
}
