using Fenicia.Auth.Domains.Security;
using Fenicia.Common.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.DbInitializer;

internal static partial class DbInitializer
{
    private static SecurityService SecurityService { get; set; } = null!;

    internal static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DefaultContext>();
        SecurityService = scope.ServiceProvider.GetRequiredService<SecurityService>();

        await context.Database.MigrateAsync();
        Seed(context);
    }

    private static void Seed(DefaultContext context)
    {
        SeedRoles(context);
        context.SaveChanges();
        SeedStates(context);
        context.SaveChanges();
        SeedModules(context);
        context.SaveChanges();
        SeedCompanies(context);
        context.SaveChanges();
        SeedAddresses(context);
        context.SaveChanges();
        SeedCompanyAddress(context);
        context.SaveChanges();
        SeedPeople(context);
        context.SaveChanges();
        SeedPosition(context);
        context.SaveChanges();
        SeedSuppliers(context);
        context.SaveChanges();
        SeedCustomers(context);
        context.SaveChanges();
        SeedEmployees(context);
        context.SaveChanges();
        SeedProductCategories(context);
        context.SaveChanges();
        SeedProducts(context);
        context.SaveChanges();
        SeedOrders(context);
        context.SaveChanges();
        SeedOrderDetails(context);
        context.SaveChanges();
        SeedStockMovements(context);
        context.SaveChanges();
        SeedPersonAddresses(context);
        context.SaveChanges();
        SeedUser(context);
        context.SaveChanges();
        SeedAuthOrders(context);
        context.SaveChanges();
        SeedAuthOrderDetails(context);
        context.SaveChanges();
        SeedSubscriptions(context);
        context.SaveChanges();
        SeedSubscriptionCredits(context);
        context.SaveChanges();
        SeedConfigurations(context);
        context.SaveChanges();
        SeedNotifications(context);
        context.SaveChanges();
        SeedForgottenPasswords(context);
        context.SaveChanges();
        SeedUploads(context);
        context.SaveChanges();
        SeedProfiles(context);
        context.SaveChanges();
        SeedFeeds(context);
        context.SaveChanges();
        SeedSocialComments(context);
        context.SaveChanges();
        SeedSocialLikes(context);
        context.SaveChanges();
        SeedSocialShares(context);
        context.SaveChanges();
        SeedSocialBlocks(context);
        context.SaveChanges();
        SeedSocialReports(context);
        context.SaveChanges();
        SeedSocialAttachments(context);
        context.SaveChanges();
        SeedProjects(context);
        context.SaveChanges();
        SeedProjectStatuses(context);
        context.SaveChanges();
        SeedSprints(context);
        context.SaveChanges();
        SeedTasks(context);
        context.SaveChanges();
        SeedProjectSubtasks(context);
        context.SaveChanges();
        SeedTaskAssignees(context);
        context.SaveChanges();
        SeedProjectComments(context);
        context.SaveChanges();
        SeedProjectAttachments(context);
        context.SaveChanges();
        SeedTeams(context);
        context.SaveChanges();
        SeedTeamUsers(context);
        context.SaveChanges();
    }
}
