using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Fenicia.Common.Data.Requests.SocialNetwork;

namespace Fenicia.Common.Data.Models.SocialNetwork;

public class UserModel(UserRequest request) : BaseModel
{
    public Guid UserId { get; set; } = request.UserId;

    [Required]
    [MaxLength(48)]
    public string Username { get; set; } = request.Username;

    [Required]
    [MaxLength(48)]
    public string Name { get; set; } = request.Name;

    [EmailAddress]
    [Required]
    [MaxLength(48)]
    public string Email { get; set; } = request.Email;

    [MaxLength(48)]
    public string? ImageUrl { get; set; } = request.ImageUrl;

    [InverseProperty(nameof(FollowerModel.Follower))]
    public List<FollowerModel> Followers { get; set; } = [];

    [InverseProperty(nameof(FollowerModel.User))]
    public List<FollowerModel> Following { get; set; } = [];
}