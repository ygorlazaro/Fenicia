using Fenicia.Common.Data.Models.SocialNetworkModels;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Common.Data.Contexts;

public partial class DefaultContext
{
    public DbSet<FeedModel> SocialNetworkFeeds { get; set; }

    public DbSet<FollowerModel> SocialNetworkFollowers { get; set; }
}
