using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.SocialNetwork;
using Fenicia.Common.Data.Repositories;
using Fenicia.Module.SocialNetwork.Domains.Friendship.Interfaces;

namespace Fenicia.Module.SocialNetwork.Domains.Friendship;

public class FriendshipRepository(DefaultContext context) : Repository<FriendshipModel>(context), IFriendshipRepository
{
    public override IQueryable<FriendshipModel> Query()
    {
        return DbSet;
    }
}
