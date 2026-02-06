using Fenicia.Common.Data.Models.SocialNetwork;
using Fenicia.Common.Data.Requests.SocialNetwork;
using Fenicia.Common.Data.Responses.SocialNetwork;

namespace Fenicia.Common.Data.Converters.SocialNetwork;

public class FeedConverter
{
    public static List<FeedResponse> Convert(List<FeedModel> model)
    {
        return model.Select(Convert).ToList();
    }

    public static FeedResponse Convert(FeedModel model)
    {
        return new FeedResponse
        {
            Id = model.Id,
            Date = model.Date,
            Text = model.Text,
            UserId = model.UserId
        };
    }

    public static FeedModel Convert(FeedRequest request)
    {
        return new FeedModel
        {
            Date = request.Date,
            Text = request.Text,
            Id = request.Id,
            UserId = request.UserId
        };
    }
}
