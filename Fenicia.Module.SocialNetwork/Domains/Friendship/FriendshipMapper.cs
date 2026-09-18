using Fenicia.Common.Data.Models.SocialNetwork;
using Fenicia.Common.DTOs.SocialNetwork.Friendship;
using Riok.Mapperly.Abstractions;

namespace Fenicia.Module.SocialNetwork.Domains.Friendship;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class FriendshipMapper
{
    public partial AddFriendshipResponse MapToAddFriendshipResponse(FriendshipModel friendship);

    public partial GetFollowersResponse MapToGetFollowersResponse(FriendshipModel friendship);

    public partial GetFollowingResponse MapToGetFollowingResponse(FriendshipModel friendship);
}
