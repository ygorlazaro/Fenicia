using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fenicia.Common.Data.Models.SocialNetwork;

public class UserModel: BaseModel
{
    public Guid UserId { get; set; }

    [Required]
    [MaxLength(48)]
    public string Username
    {
        get;
        set;
    }

    [Required]
    [MaxLength(48)]
    public string Name
    {
        get;
        set;
    }

    [EmailAddress]
    [Required]
    [MaxLength(48)]
    public string Email
    {
        get;
        set;
    }

    [MaxLength(48)]
    public string ImageUrl
    {
        get;
        set;
    }

    [InverseProperty(nameof(FollowerModel.Follower))]
    public List<FollowerModel> Followers { get; set; }

    [InverseProperty(nameof(FollowerModel.User))]
    public List<FollowerModel> Following { get; set; }
}
