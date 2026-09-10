using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.SocialNetwork;
using Fenicia.Common.Data.Repositories;

namespace Fenicia.Module.SocialNetwork.Domains.Friendship;

public class FriendshipRepository(DefaultContext context) : Repository<FriendshipModel>(context), IFriendshipRepository
{
    public new IQueryable<FriendshipModel> Query() => DbSet;
}
