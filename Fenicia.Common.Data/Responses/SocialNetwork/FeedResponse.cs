using Fenicia.Common.Data.Models.SocialNetwork;

namespace Fenicia.Common.Data.Responses.SocialNetwork;

public class FeedResponse(FeedModel model)
{
    public Guid Id { get; set; } = model.Id;

    public DateTime Date { get; set; } = model.Date;

    public string Text { get; set; } = model.Text;

    public Guid UserId { get; set; } = model.UserId;
}