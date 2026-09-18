using Fenicia.Common.Data.Models.SocialNetwork;
using Fenicia.Common.DTOs.SocialNetwork.Feed;
using Riok.Mapperly.Abstractions;

namespace Fenicia.Module.SocialNetwork.Domains.Feed;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class FeedMapper
{
    [MapProperty("Profile.UserName", nameof(GetAllFeedResponse.AuthorUserName))]
    [MapProperty("Profile.Upload.Url", nameof(GetAllFeedResponse.AuthorImageUrl))]
    public partial GetAllFeedResponse MapToGetAllFeedResponse(FeedModel feed);

    [MapProperty("Profile.UserName", nameof(GetFeedByIdResponse.AuthorUserName))]
    [MapProperty("Profile.Upload.Url", nameof(GetFeedByIdResponse.AuthorImageUrl))]
    public partial GetFeedByIdResponse MapToGetFeedByIdResponse(FeedModel feed);

    public partial AddFeedResponse MapToAddFeedResponse(FeedModel feed);

    public partial UpdateFeedResponse MapToUpdateFeedResponse(FeedModel feed);
}
