using Fenicia.Common.Data.Models.SocialNetworkModels;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Common.Data.Contexts;

#pragma warning disable SA1601 // Partial elements should be documented
public partial class DefaultContext
#pragma warning restore SA1601 // Partial elements should be documented
{
    public DbSet<FeedModel> SocialNetworkFeeds { get; set; }

    public DbSet<FollowerModel> SocialNetworkFollowers { get; set; }
}
