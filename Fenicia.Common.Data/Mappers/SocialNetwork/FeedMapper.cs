using Fenicia.Common.Data.Models.SocialNetwork;
using Fenicia.Common.Data.Requests.SocialNetwork;
using Fenicia.Common.Data.Responses.SocialNetwork;

namespace Fenicia.Common.Data.Mappers.SocialNetwork;

public class FeedMapper
{
    public static List<FeedResponse> Map(List<FeedModel> model)
    {
        return [.. model.Select(Map)];
    }

    public static FeedResponse Map(FeedModel model)
    {
        return new FeedResponse
        {
            Id = model.Id,
            Date = model.Date,
            Text = model.Text,
            UserId = model.UserId
        };
    }

    public static FeedModel Map(FeedRequest request)
    {
        return new FeedModel
        {
            Date = DateTime.Now,
            Text = request.Text,
            Id = request.Id,
            UserId = request.UserId
        };
    }
}