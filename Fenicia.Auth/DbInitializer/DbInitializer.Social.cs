using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.SocialNetwork;
using Fenicia.Common.Enums.SocialNetwork;

namespace Fenicia.Auth.DbInitializer;

internal static partial class DbInitializer
{
    private static void SeedFeeds(DefaultContext context)
    {
        if (context.SocialNetworkFeeds.Any())
        {
            return;
        }

        var companyId = context.AuthCompanies.Select(x => x.Id).FirstOrDefault();
        var profiles = context.AuthProfiles.ToList();

        if (!profiles.Any())
        {
            return;
        }

        var random = new Random();
        var now = DateTime.UtcNow;

        for (var i = 1; i <= 200; i++)
        {
            var profile = profiles[(i - 1) % profiles.Count];

            var text = (i % 5) switch
            {
                0 => $"Post {i} - Aprendendo novas tecnologias!",
                1 => $"Post {i} - Finalizando sprint com sucesso!",
                2 => $"Post {i} - Novo projeto na area!",
                3 => $"Post {i} - Reflexao sobre agile...",
                _ => $"Post {i} - Compartilhando conhecimento!"
            };

            var feed = new FeedModel
            {
                CompanyId = companyId,
                ProfileId = profile.Id,
                Text = text,
                Date = now.AddDays(-(random.NextDouble() * 365)),
                TotalLikes = (int)(random.NextDouble() * 100),
                TotalComments = (int)(random.NextDouble() * 30),
                TotalShares = (int)(random.NextDouble() * 20),
                Created = now.AddDays(-random.NextDouble() * 365),
                Updated = now.AddDays(-random.NextDouble() * 30)
            };

            context.SocialNetworkFeeds.Add(feed);
        }
    }

    private static void SeedSocialComments(DefaultContext context)
    {
        if (context.SocialNetworkComments.Any())
        {
            return;
        }

        var companyId = context.AuthCompanies.Select(x => x.Id).FirstOrDefault();
        var profiles = context.AuthProfiles.ToList();
        var feeds = context.SocialNetworkFeeds.ToList();

        if (!profiles.Any() || !feeds.Any())
        {
            return;
        }

        var random = new Random();
        var now = DateTime.UtcNow;

        for (var i = 1; i <= 300; i++)
        {
            var profile = profiles[(i - 1) % profiles.Count];
            var feed = feeds[(i - 1) % feeds.Count];

            var text = (i % 3) switch
            {
                0 => $"Comentário {i} — Concordo totalmente!",
                1 => $"Comentário {i} — Interessante ponto de vista.",
                _ => $"Comentário {i} — Obrigado por compartilhar!"
            };

            var comment = new CommentModel
            {
                CompanyId = companyId,
                ProfileId = profile.Id,
                FeedId = feed.Id,
                Text = text,
                CommentDate = now.AddDays(-(random.NextDouble() * 365)),
                UpdatedDate = i % 4 == 0 ? now.AddDays(-(random.NextDouble() * 30)) : null,
                Created = now.AddDays(-random.NextDouble() * 365),
                Updated = now.AddDays(-random.NextDouble() * 30)
            };

            context.SocialNetworkComments.Add(comment);
        }
    }

    private static void SeedSocialLikes(DefaultContext context)
    {
        if (context.SocialNetworkLikes.Any())
        {
            return;
        }

        var companyId = context.AuthCompanies.Select(x => x.Id).FirstOrDefault();
        var profiles = context.AuthProfiles.ToList();
        var feeds = context.SocialNetworkFeeds.ToList();

        if (!profiles.Any() || !feeds.Any())
        {
            return;
        }

        var random = new Random();
        var now = DateTime.UtcNow;

        for (var i = 1; i <= 400; i++)
        {
            var like = new LikeModel
            {
                CompanyId = companyId,
                ProfileId = profiles[(i - 1) % profiles.Count].Id,
                FeedId = feeds[(i - 1) % feeds.Count].Id,
                LikeDate = now.AddDays(-(random.NextDouble() * 365)),
                Created = now.AddDays(-random.NextDouble() * 30),
                Updated = now.AddDays(-random.NextDouble() * 30)
            };

            context.SocialNetworkLikes.Add(like);
        }
    }

    private static void SeedSocialShares(DefaultContext context)
    {
        if (context.SocialNetworkShares.Any())
        {
            return;
        }

        var companyId = context.AuthCompanies.Select(x => x.Id).FirstOrDefault();
        var profiles = context.AuthProfiles.ToList();
        var feeds = context.SocialNetworkFeeds.ToList();

        if (!profiles.Any() || !feeds.Any())
        {
            return;
        }

        var random = new Random();
        var now = DateTime.UtcNow;

        for (var i = 1; i <= 50; i++)
        {
            var share = new ShareModel
            {
                CompanyId = companyId,
                ProfileId = profiles[(i - 1) % profiles.Count].Id,
                OriginalFeedId = feeds[(i - 1) % feeds.Count].Id,
                Text = i % 2 == 0 ? "Compartilhando com meu comentário..." : null,
                ShareDate = now.AddDays(-(random.NextDouble() * 365)),
                Created = now.AddDays(-random.NextDouble() * 30),
                Updated = now.AddDays(-random.NextDouble() * 30)
            };

            context.SocialNetworkShares.Add(share);
        }
    }

    private static void SeedSocialBlocks(DefaultContext context)
    {
        if (context.SocialNetworkBlocks.Any())
        {
            return;
        }

        var profiles = context.AuthProfiles.ToList();

        if (!profiles.Any())
        {
            return;
        }

        var random = new Random();
        var now = DateTime.UtcNow;

        for (var i = 1; i <= 10; i++)
        {
            var profileIndex = (i - 1) % profiles.Count;
            var blockedProfileIndex = (((i - 1) % 100) + 1) % 100;

            var reason = (i % 3) switch
            {
                0 => "Conteúdo impróprio",
                1 => "Spam",
                _ => "Assédio"
            };

            var block = new BlockModel
            {
                ProfileId = profiles[profileIndex].Id,
                BlockedProfileId = profiles[blockedProfileIndex].Id,
                Reason = reason,
                BlockDate = now.AddDays(-(random.NextDouble() * 180)),
                IsActive = i % 2 == 0,
                Created = now.AddDays(-random.NextDouble() * 180),
                Updated = now.AddDays(-random.NextDouble() * 30)
            };

            context.SocialNetworkBlocks.Add(block);
        }
    }

    private static void SeedSocialReports(DefaultContext context)
    {
        if (context.SocialNetworkReports.Any())
        {
            return;
        }

        var users = context.AuthUsers.ToList();
        var profiles = context.AuthProfiles.ToList();

        if (!users.Any() || !profiles.Any())
        {
            return;
        }

        var random = new Random();
        var now = DateTime.UtcNow;

        for (var i = 1; i <= 20; i++)
        {
            var targetType = i % 2 == 0 ? "profile" : "feed";
            var targetId = profiles[(i - 1) % profiles.Count].Id;

            var reason = (i % 3) switch
            {
                0 => "Spam",
                1 => "Conteúdo ofensivo",
                _ => "Assédio"
            };

            var report = new ReportModel
            {
                ReporterId = users[(i - 1) % users.Count].Id,
                TargetId = targetId,
                TargetType = targetType,
                Reason = reason,
                Description = $"Descrição detalhada da denúncia {i}",
                Status = (EnumReportStatus)(i % 3),
                ReportDate = now.AddDays(-(random.NextDouble() * 180)),
                Created = now.AddDays(-random.NextDouble() * 180),
                Updated = now.AddDays(-random.NextDouble() * 30)
            };

            context.SocialNetworkReports.Add(report);
        }
    }

    private static void SeedSocialAttachments(DefaultContext context)
    {
        if (context.SocialNetworkAttachments.Any())
        {
            return;
        }

        var companyId = context.AuthCompanies.Select(x => x.Id).FirstOrDefault();
        var comments = context.SocialNetworkComments.ToList();

        if (!comments.Any())
        {
            return;
        }

        var random = new Random();
        var now = DateTime.UtcNow;

        for (var i = 1; i <= 50; i++)
        {
            var fileSize = (long)((random.NextDouble() * 2000000) + 100000);

            var attachment = new AttachmentModel
            {
                CompanyId = companyId,
                CommentId = comments[(i - 1) % comments.Count].Id,
                Url = $"https://fenicia.s3.amazonaws.com/social/attachment-{i}.jpg",
                FileType = "image/jpeg",
                FileSize = fileSize,
                UploadDate = now.AddDays(-(random.NextDouble() * 365)),
                Created = now.AddDays(-random.NextDouble() * 30),
                Updated = now.AddDays(-random.NextDouble() * 30)
            };

            context.SocialNetworkAttachments.Add(attachment);
        }
    }
}
