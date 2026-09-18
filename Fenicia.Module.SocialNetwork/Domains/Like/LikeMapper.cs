using Fenicia.Common.Data.Models.SocialNetwork;
using Fenicia.Common.DTOs.SocialNetwork.Like;
using Riok.Mapperly.Abstractions;

namespace Fenicia.Module.SocialNetwork.Domains.Like;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class LikeMapper
{
    public partial AddLikeResponse MapToAddLikeResponse(LikeModel like);

    public partial GetLikesResponse MapToGetLikesResponse(LikeModel like);

    public partial GetLikedFeedsResponse MapToGetLikedFeedsResponse(FeedModel feed);
}
