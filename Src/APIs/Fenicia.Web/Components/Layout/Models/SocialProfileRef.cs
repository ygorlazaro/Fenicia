namespace Fenicia.Web.Components.Layout.Models;

public class SocialProfileRef
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string? UserName { get; set; }

    public string? Bio { get; set; }

    public string? ImageUrl { get; set; }

    public Guid? UploadId { get; set; }

    public string? Website { get; set; }

    public string? Location { get; set; }

    public string? Phone { get; set; }

    public DateTime? BirthDate { get; set; }

    public DateTime Created { get; set; }

    public DateTime? Updated { get; set; }

    public int PostsCount { get; set; }

    public int FollowersCount { get; set; }

    public int FollowingCount { get; set; }
}
