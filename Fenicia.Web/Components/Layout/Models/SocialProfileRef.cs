namespace Fenicia.Web.Components.Layout.Models;

public class SocialProfileRef
{
    public Guid Id { get; init; }

    public Guid UserId { get; init; }

    public string? UserName { get; init; }

    public string? Bio { get; init; }

    public string? ImageUrl { get; init; }

    public Guid? UploadId { get; init; }

    public string? Website { get; init; }

    public string? Location { get; init; }

    public string? Phone { get; init; }

    public DateTime? BirthDate { get; init; }

    public DateTime Created { get; init; }

    public DateTime? Updated { get; init; }

    public int PostsCount { get; init; }

    public int FollowersCount { get; init; }

    public int FollowingCount { get; init; }
}
