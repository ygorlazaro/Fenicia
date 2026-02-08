using Fenicia.Common.Data.Models.SocialNetwork;

namespace Fenicia.Common.Data.Responses.SocialNetwork;

public class FollowerResponse(FollowerModel model)
{
    public Guid Id { get; set; } = model.Id;

    public Guid UserId { get; set; } = model.UserId;

    public Guid FollowerId { get; set; } = model.FollowerId;

    public DateTime FollowDate { get; set; } = model.FollowDate;

    public bool IsActive { get; set; } = model.IsActive;
}