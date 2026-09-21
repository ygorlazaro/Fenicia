using Fenicia.Common.Data.Models.SocialNetwork;
using Fenicia.Common.DTOs.SocialNetwork.Feed;
using Fenicia.Common.DTOs.SocialNetwork.Share;
using Fenicia.Module.SocialNetwork.Domains.Feed.Interfaces;
using Fenicia.Module.SocialNetwork.Domains.Share.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.SocialNetwork.Domains.Share;

public class ShareService(IShareRepository repository, IFeedService feedService) : IShareService
{
    public async Task<AddShareResponse> ShareAsync(
        ShareCommand command,
        Guid companyId,
        Guid profileId,
        CancellationToken cancellationToken = default)
    {
        _ = await feedService.GetByIdAsync(new GetFeedByIdQuery(command.OriginalFeedId), cancellationToken)
            ?? throw new InvalidOperationException("Post original não encontrado.");

        var model = new ShareModel
        {
            Id = command.Id,
            ProfileId = profileId,
            OriginalFeedId = command.OriginalFeedId,
            Text = command.Text,
            CompanyId = companyId,
            ShareDate = DateTime.UtcNow
        };

        var created = await repository.InsertAsync(model, cancellationToken);

        var shareFeed = new FeedModel
        {
            Id = Guid.NewGuid(),
            Date = created.ShareDate,
            Text = command.Text ?? string.Empty,
            ProfileId = profileId,
            OriginalFeedId = command.OriginalFeedId,
            CompanyId = companyId,
            TotalLikes = 0,
            TotalComments = 0,
            TotalShares = 0
        };
        await feedService.AddAsync(new FeedRequest(
            shareFeed.Id,
            shareFeed.Date,
            shareFeed.Text,
            shareFeed.ProfileId,
            shareFeed.OriginalFeedId),
            companyId,
            cancellationToken);

        await feedService.IncrementTotalSharesAsync(command.OriginalFeedId, cancellationToken);

        return new AddShareResponse(
            created.Id,
            created.OriginalFeedId,
            created.Text,
            created.CompanyId,
            created.ProfileId,
            created.ShareDate,
            shareFeed.Id);
    }

    public async Task<List<GetSharesResponse>> GetSharesByFeedAsync(
        GetSharesByFeedQuery query,
        Guid feedId,
        CancellationToken cancellationToken = default)
    {
        var baseQuery = repository.Query().Where(s => s.OriginalFeedId == feedId);
        var shares = await baseQuery.Skip((query.Page - 1) * query.PerPage).Take(query.PerPage)
            .ToListAsync(cancellationToken);
        return [.. shares.Select(s => new GetSharesResponse(
            s.Id,
            s.OriginalFeedId,
            s.Text,
            s.CompanyId,
            s.ProfileId,
            s.ShareDate))];
    }
}
