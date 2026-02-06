using Fenicia.Common.Data.Models.SocialNetwork;
using Fenicia.Common.Data.Responses.SocialNetwork;

namespace Fenicia.Common.Data.Converters.SocialNetwork;

public class FollowerConverter
{
    public static FollowerResponse Convert(FollowerModel model)
    {
        return new FollowerResponse()
        {
            Id = model.Id,
            UserId = model.UserId,
            FollowerId = model.FollowerId,
            FollowDate = model.FollowDate,
            IsActive = model.IsActive
        };
    }

    public static List<FollowerResponse> Convert(List<FollowerModel> models)
    {
        return models.Select(Convert).ToList();
    }
}
