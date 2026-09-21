using Fenicia.Common.API;
using Fenicia.Common.API.Startup;
using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.DTOs.Auth.Upload;
using Fenicia.Module.SocialNetwork.Domains.Attachment;
using Fenicia.Module.SocialNetwork.Domains.Attachment.Interfaces;
using Fenicia.Module.SocialNetwork.Domains.Block;
using Fenicia.Module.SocialNetwork.Domains.Block.Interfaces;
using Fenicia.Module.SocialNetwork.Domains.Comment;
using Fenicia.Module.SocialNetwork.Domains.Comment.Interfaces;
using Fenicia.Module.SocialNetwork.Domains.Feed;
using Fenicia.Module.SocialNetwork.Domains.Feed.Interfaces;
using Fenicia.Module.SocialNetwork.Domains.Friendship;
using Fenicia.Module.SocialNetwork.Domains.Friendship.Interfaces;
using Fenicia.Module.SocialNetwork.Domains.Like;
using Fenicia.Module.SocialNetwork.Domains.Like.Interfaces;
using Fenicia.Module.SocialNetwork.Domains.Profile;
using Fenicia.Module.SocialNetwork.Domains.Profile.Interfaces;
using Fenicia.Module.SocialNetwork.Domains.Report;
using Fenicia.Module.SocialNetwork.Domains.Report.Interfaces;
using Fenicia.Module.SocialNetwork.Domains.Share;
using Fenicia.Module.SocialNetwork.Domains.Share.Interfaces;

namespace Fenicia.Module.SocialNetwork;

public class Program
{
    public static void Main(string[] args)
    {
        FeniciaModuleLoader.Load(args, out var configuration, out var builder);

        builder.AddFeniciaLogging().AddFeniciaRateLimiting(configuration).AddFeniciaCors()
            .AddFeniciaAuthentication(configuration).AddFeniciaControllers().AddFeniciaLocalization()
            .AddFeniciaDependencyInjection(() =>
            {
                builder.Services.AddSingleton<ICompanyContext, CompanyContext>();
                builder.Services.AddHttpContextAccessor();
                
                // Profile
                builder.Services.AddScoped<IProfileRepository, ProfileRepository>();
                builder.Services.AddScoped<IProfileService, ProfileService>();
                
                // Feed
                builder.Services.AddScoped<IFeedRepository, FeedRepository>();
                builder.Services.AddScoped<IFeedService, FeedService>();
                
                // Like
                builder.Services.AddScoped<ILikeRepository, LikeRepository>();
                builder.Services.AddScoped<ILikeService, LikeService>();
                
                // Comment
                builder.Services.AddScoped<ICommentRepository, CommentRepository>();
                builder.Services.AddScoped<ICommentService, CommentService>();
                
                // Attachment
                builder.Services.AddScoped<IAttachmentRepository, AttachmentRepository>();
                builder.Services.AddScoped<IAttachmentService, AttachmentService>();
                
                // Share
                builder.Services.AddScoped<IShareRepository, ShareRepository>();
                builder.Services.AddScoped<IShareService, ShareService>();
                
                // Report
                builder.Services.AddScoped<IReportRepository, ReportRepository>();
                builder.Services.AddScoped<IReportService, ReportService>();
                
                // Block
                builder.Services.AddScoped<IBlockRepository, BlockRepository>();
                builder.Services.AddScoped<IBlockService, BlockService>();
                
                // Friendship
                builder.Services.AddScoped<IFriendshipRepository, FriendshipRepository>();
                builder.Services.AddScoped<IFriendshipService, FriendshipService>();
                
                builder.Services.Configure<UploadOptions>(configuration.GetSection("Upload"));
            }).AddFeniciaDbContext<DefaultContext>(configuration, "Fenicia.Auth", "Auth");

        var app = builder.Build();
        app.UseFeniciaLocalization();

        if (Environment.GetEnvironmentVariable("ASPNETCORE_TESTING") == "true")
        {
            return;
        }

        app.UseCors(app.Environment.IsDevelopment() ? "DevCors" : "RestrictedCors");
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}