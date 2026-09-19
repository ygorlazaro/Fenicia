using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.Token;

public class TokenResponse()
{
    public TokenResponse(Guid userId, string name, string email, Guid companyId, List<string> roles)
        : this()
    {
        UserId = userId;
        Name = name;
        Email = email;
        CompanyId = companyId;
        Roles = roles;
    }

    [Required]
    public Guid UserId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    public Guid CompanyId { get; set; }

    public List<string> Roles { get; } = [];

    public string Token { get; set; } = string.Empty;

    public string RefreshToken { get; set; } = string.Empty;
}
