using Fenicia.Auth.Contexts.Models;
using Fenicia.Common.Database;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Contexts;

public class AuthContext(DbContextOptions<AuthContext> options) : DbContext(options)
{
    public DbSet<RoleModel> Roles { get; set; } = null!;

    public DbSet<UserModel> Users { get; set; } = null!;

    public DbSet<UserRoleModel> UserRoles { get; set; } = null!;

    public DbSet<CompanyModel> Companies { get; set; } = null!;

    public DbSet<ModuleModel> Modules { get; set; } = null!;

    public DbSet<CustomerModel> Customers { get; set; } = null!;

    public DbSet<OrderModel> Orders { get; set; } = null!;

    public DbSet<OrderDetailModel> OrderDetails { get; set; } = null!;

    public DbSet<SubscriptionModel> Subscriptions { get; set; } = null!;

    public DbSet<SubscriptionCreditModel> SubscriptionCredits { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        PostgresDateTimeOffsetSupport.Init(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }
}