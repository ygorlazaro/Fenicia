using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.Subscription;

public class GetUserProfileQuery()
{
    public GetUserProfileQuery(Guid userId)
        : this()
    {
        UserId = userId;
    }

    [Required]
    public Guid UserId { get; set; }
}
