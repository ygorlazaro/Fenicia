using Fenicia.Common.Data.Models.Auth;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Common.Data.Contexts;

public partial class DefaultContext
{
    public DbSet<ConfigurationModel> AuthConfigurations { get; set; }

    public DbSet<RoleModel> AuthRoles { get; set; } = null!;

    public DbSet<UserModel> AuthUsers { get; set; } = null!;

    public DbSet<UserRoleModel> AuthUserRoles { get; set; } = null!;

    public DbSet<CompanyModel> AuthCompanies { get; set; } = null!;

    public DbSet<ModuleModel> AuthModules { get; set; } = null!;

    public DbSet<OrderModel> AuthOrders { get; set; } = null!;

    public DbSet<OrderDetailModel> AuthOrderDetails { get; set; } = null!;

    public DbSet<SubscriptionModel> AuthSubscriptions { get; set; } = null!;

    public DbSet<SubscriptionCreditModel> AuthSubscriptionCredits { get; set; } = null!;

    public DbSet<AddressModel> AuthAddresses { get; set; } = null!;

    public DbSet<StateModel> AuthStates { get; set; } = null!;

    public DbSet<ForgotPasswordModel> AuthForgottenPasswords { get; set; } = null!;

    public DbSet<NotificationModel> AuthNotifications { get; set; } = null!;
}
