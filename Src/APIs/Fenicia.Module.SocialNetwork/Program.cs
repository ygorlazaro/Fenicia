using Fenicia.Common.API;
using Fenicia.Common.API.Startup;
using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Module.SocialNetwork.Domains.Attachment;
using Fenicia.Module.SocialNetwork.Domains.Block;
using Fenicia.Module.SocialNetwork.Domains.Comment;
using Fenicia.Module.SocialNetwork.Domains.Feed;
using Fenicia.Module.SocialNetwork.Domains.Friendship;
using Fenicia.Module.SocialNetwork.Domains.Like;
using Fenicia.Module.SocialNetwork.Domains.Profile;
using Fenicia.Module.SocialNetwork.Domains.Profile.Interfaces;
using Fenicia.Module.SocialNetwork.Domains.Report;
using Fenicia.Module.SocialNetwork.Domains.Share;

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
                builder.Services.AddScoped<IProfileRepository, ProfileRepository>();
                builder.Services.AddScoped<IProfileService, ProfileService>();
                builder.Services.AddScoped<FeedRepository>();
                builder.Services.AddScoped<LikeRepository>();
                builder.Services.AddScoped<CommentRepository>();
                builder.Services.AddScoped<AttachmentRepository>();
                builder.Services.AddScoped<ShareRepository>();
                builder.Services.AddScoped<ReportRepository>();
                builder.Services.AddScoped<IBlockRepository, BlockRepository>();
                builder.Services.AddScoped<IFriendshipRepository, FriendshipRepository>();
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
        app.Run();
    }
}