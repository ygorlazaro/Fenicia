using Fenicia.Common.Data.Models.SocialNetwork;
using Fenicia.Common.Data.Responses.SocialNetwork;

namespace Fenicia.Common.Data.Mappers.SocialNetwork;

public class FollowerMapper
{
    public static FollowerResponse Map(FollowerModel model)
    {
        return new FollowerResponse
        {
            Id = model.Id,
            UserId = model.UserId,
            FollowerId = model.FollowerId,
            FollowDate = model.FollowDate,
            IsActive = model.IsActive
        };
    }

    public static List<FollowerResponse> Map(List<FollowerModel> models)
    {
        return [.. models.Select(Map)];
    }
}