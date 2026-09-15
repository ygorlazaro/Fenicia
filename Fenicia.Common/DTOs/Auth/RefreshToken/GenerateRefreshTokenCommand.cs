using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.RefreshToken;

public class GenerateRefreshTokenCommand()
{
    public GenerateRefreshTokenCommand(Guid userId)
        : this()
    {
        UserId = userId;
    }

    [Required]
    public Guid UserId { get; set; }
}
