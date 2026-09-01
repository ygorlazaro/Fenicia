using Fenicia.Common.Data.Models.SocialNetwork;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Common.Data.Contexts;

#pragma warning disable SA1601 // Partial elements should be documented
public partial class DefaultContext
#pragma warning restore SA1601 // Partial elements should be documented
{
    public DbSet<FeedModel> SocialNetworkFeeds { get; set; }

    public DbSet<ProfileModel> SocialNetworkProfiles { get; set; }

    public DbSet<FriendshipModel> SocialNetworkFriendships { get; set; }

    public DbSet<BlockModel> SocialNetworkBlocks { get; set; }

    public DbSet<CommentModel> SocialNetworkComments { get; set; }

    public DbSet<LikeModel> SocialNetworkLikes { get; set; }

    public DbSet<AttachmentModel> SocialNetworkAttachments { get; set; }

    public DbSet<ShareModel> SocialNetworkShares { get; set; }

    public DbSet<ReportModel> SocialNetworkReports { get; set; }
}
