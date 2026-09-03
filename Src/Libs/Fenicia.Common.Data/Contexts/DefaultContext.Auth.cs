using Fenicia.Common.Data.Models.Auth;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Common.Data.Contexts;

#pragma warning disable SA1601 // Partial elements should be documented
public partial class DefaultContext
#pragma warning restore SA1601 // Partial elements should be documented
{
    public DbSet<ConfigurationModel> AuthConfigurations { get; set; }

    public DbSet<RoleModel> AuthRoles { get; set; }

    public DbSet<UserModel> AuthUsers { get; set; }

    public DbSet<UserRoleModel> AuthUserRoles { get; set; }

    public DbSet<CompanyModel> AuthCompanies { get; set; }

    public DbSet<ModuleModel> AuthModules { get; set; }

    public DbSet<OrderModel> AuthOrders { get; set; }

    public DbSet<OrderDetailModel> AuthOrderDetails { get; set; }

    public DbSet<SubscriptionModel> AuthSubscriptions { get; set; }

    public DbSet<SubscriptionCreditModel> AuthSubscriptionCredits { get; set; }

    public DbSet<AddressModel> AuthAddresses { get; set; }

    public DbSet<StateModel> AuthStates { get; set; }

    public DbSet<ForgotPasswordModel> AuthForgottenPasswords { get; set; }

    public DbSet<NotificationModel> AuthNotifications { get; set; }
}