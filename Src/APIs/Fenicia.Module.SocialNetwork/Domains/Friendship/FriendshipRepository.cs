using Fenicia.Common.Data.Models.SocialNetwork;
using Fenicia.Common.Data.Repositories;

namespace Fenicia.Module.SocialNetwork.Domains.Friendship;

public interface IFriendshipRepository : IRepository<FriendshipModel>
{
    new IQueryable<FriendshipModel> Query();
}