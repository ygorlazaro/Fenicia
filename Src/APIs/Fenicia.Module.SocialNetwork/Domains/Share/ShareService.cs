using Fenicia.Common;
using Fenicia.Common.Data.Models.SocialNetwork;
using Fenicia.Module.SocialNetwork.Domains.Share.DTOs;
using Microsoft.EntityFrameworkCore;

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
        var baseQuery = repository.Query().Where(s => s.OriginalFeedId == feedId);
        var filters = AdvancedQueryParser.Parse(query.Query);
        var filteredQuery = baseQuery.ApplyAdvancedQuery(filters, query.Sort);
        var shares = await filteredQuery.Skip((query.Page - 1) * query.PerPage).Take(query.PerPage).ToListAsync(cancellationToken);
        return [.. shares.Select(s => new GetSharesResponse(s.Id, s.OriginalFeedId, s.Text, s.CompanyId, s.UserId, s.ShareDate))];
    }
}
