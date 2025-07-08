namespace Fenicia.Auth.Domains.User.Data;

using System.ComponentModel.DataAnnotations;

public class UserResponse
{
    [Required]
    [StringLength(maximumLength: 48, MinimumLength = 2)]
    [Display(Name = "Full Name")]
    public string Name { get; set; } = null!;

    [Required]
    [EmailAddress]
    [StringLength(maximumLength: 48)]
    [Display(Name = "Email Address")]
    public string Email { get; set; } = null!;

    [Required]
    [Display(Name = "User ID")]
    public Guid Id
    {
        get; set;
    }

    public static UserResponse Convert(UserModel user)
    {
        return new UserResponse { Id = user.Id, Name = user.Name, Email = user.Email };
    }
}
