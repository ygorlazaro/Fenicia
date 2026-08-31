using Fenicia.Common.Data.Models.SocialNetworkModels;
using Fenicia.Module.SocialNetwork.Domains.Share.DTOs;

namespace Fenicia.Module.SocialNetwork.Domains.Share;

public class ShareService(ShareRepository repository)
{
    public async Task<AddShareResponse> ShareAsync(ShareCommand command, Guid companyId, Guid userId, CancellationToken cancellationToken = default)
    {
        var model = new ShareModel
        {
            Id = command.Id,
            UserId = userId,
            OriginalFeedId = command.OriginalFeedId,
            Text = command.Text,
            CompanyId = companyId,
            ShareDate = DateTime.UtcNow
        };

        var created = await repository.InsertAsync(model, cancellationToken);
        return new AddShareResponse(created.Id, created.OriginalFeedId, created.Text, created.CompanyId, created.UserId, created.ShareDate);
    }

    public async Task<List<GetSharesResponse>> GetSharesByFeedAsync(GetSharesByFeedQuery query, Guid feedId, CancellationToken cancellationToken = default)
    {
        var shares = await repository.GetSharesByFeedAsync(query.Page, query.PerPage, feedId, cancellationToken);
        return [.. shares.Select(s => new GetSharesResponse(s.Id, s.OriginalFeedId, s.Text, s.CompanyId, s.UserId, s.ShareDate))];
    }
}
