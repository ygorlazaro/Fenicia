using Fenicia.Common.DTOs.SocialNetwork.Share;

namespace Fenicia.Module.SocialNetwork.Domains.Share.Interfaces;

public interface IShareService
{
    Task<AddShareResponse> ShareAsync(ShareCommand command, Guid companyId, Guid profileId, CancellationToken cancellationToken = default);
    Task<List<GetSharesResponse>> GetSharesByFeedAsync(GetSharesByFeedQuery query, Guid feedId, CancellationToken cancellationToken = default);
}