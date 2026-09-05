using Fenicia.Common.Data.Models.SocialNetwork;
using Fenicia.Module.SocialNetwork.Domains.Feed;
using Fenicia.Module.SocialNetwork.Domains.Share.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.SocialNetwork.Domains.Share;

public class ShareService(ShareRepository repository, FeedRepository feedRepository)
{
    public async Task<AddShareResponse> ShareAsync(
        ShareCommand command,
        Guid companyId,
        Guid profileId,
        CancellationToken cancellationToken = default)
    {
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
        await IncrementFeedTotalSharesAsync(command.OriginalFeedId, cancellationToken);
        return new AddShareResponse(
            created.Id,
            created.OriginalFeedId,
            created.Text,
            created.CompanyId,
            created.ProfileId,
            created.ShareDate);
    }

    public async Task<List<GetSharesResponse>> GetSharesByFeedAsync(
        GetSharesByFeedQuery query,
        Guid feedId,
        CancellationToken cancellationToken = default)
    {
        var baseQuery = repository.Query().Where(s => s.OriginalFeedId == feedId);
        var filteredQuery = baseQuery;
        var shares = await filteredQuery.Skip((query.Page - 1) * query.PerPage).Take(query.PerPage)
            .ToListAsync(cancellationToken);
        return
        [
            .. shares.Select(s => new GetSharesResponse(
                s.Id,
                s.OriginalFeedId,
                s.Text,
                s.CompanyId,
                s.ProfileId,
                s.ShareDate))
        ];
    }

    private async Task IncrementFeedTotalSharesAsync(Guid feedId, CancellationToken cancellationToken)
    {
        var feed = await feedRepository.GetByIdAsync(feedId, cancellationToken);
        if (feed is null)
        {
            return;
        }

        feed.TotalShares++;
        await feedRepository.UpdateAsync(feedId, feed, cancellationToken);
    }
}
