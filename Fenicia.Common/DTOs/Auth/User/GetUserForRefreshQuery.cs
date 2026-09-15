using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.User;

public class GetUserForRefreshQuery()
{
    public GetUserForRefreshQuery(Guid userId)
        : this()
    {
        UserId = userId;
    }

    [Required]
    public Guid UserId { get; set; }
}
