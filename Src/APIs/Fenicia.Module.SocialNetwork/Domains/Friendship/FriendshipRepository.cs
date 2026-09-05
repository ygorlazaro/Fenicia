using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.SocialNetwork;
using Fenicia.Common.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.SocialNetwork.Domains.Friendship;

public interface IFriendshipRepository : IRepository<FriendshipModel>
{
    new IQueryable<FriendshipModel> Query();
}

public class FriendshipRepository(DefaultContext context) : Repository<FriendshipModel>(context), IFriendshipRepository
{
    public new IQueryable<FriendshipModel> Query() => DbSet;
}
