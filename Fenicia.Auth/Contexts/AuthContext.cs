using Fenicia.Auth.Domains.Address;
using Fenicia.Auth.Domains.Company;
using Fenicia.Auth.Domains.Module;
using Fenicia.Auth.Domains.Order;
using Fenicia.Auth.Domains.OrderDetail;
using Fenicia.Auth.Domains.RefreshToken;
using Fenicia.Auth.Domains.Role;
using Fenicia.Auth.Domains.State;
using Fenicia.Auth.Domains.Subscription;
using Fenicia.Auth.Domains.SubscriptionCredit;
using Fenicia.Auth.Domains.User;
using Fenicia.Auth.Domains.UserRole;
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

    public DbSet<OrderModel> Orders { get; set; } = null!;

    public DbSet<OrderDetailModel> OrderDetails { get; set; } = null!;

    public DbSet<SubscriptionModel> Subscriptions { get; set; } = null!;

    public DbSet<SubscriptionCreditModel> SubscriptionCredits { get; set; } = null!;

    public DbSet<AddressModel> Addresses { get; set; } = null!;

    public DbSet<StateModel> States { get; set; } = null!;

    public DbSet<RefreshTokenModel> RefreshTokens { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        PostgresDateTimeOffsetSupport.Init(modelBuilder);
        AddSoftDeleteSupport(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }

    private static void AddSoftDeleteSupport(ModelBuilder modelBuilder)
    {
        var mutableEntityTypes = modelBuilder
            .Model.GetEntityTypes()
            .Where(entityType => typeof(BaseModel).IsAssignableFrom(entityType.ClrType));

        foreach (var entityType in mutableEntityTypes)
        {
            entityType.AddSoftDeleteQueryFilter();
        }
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellation = default)
    {
        foreach (var item in ChangeTracker.Entries())
        {
            if (item.Entity is not BaseModel model)
            {
                continue;
            }

            switch (item.State)
            {
                case EntityState.Added:
                    model.Created = DateTime.UtcNow;
                    break;
                case EntityState.Modified:
                    model.Updated = DateTime.UtcNow;
                    break;
                case EntityState.Deleted:
                    model.Deleted = DateTime.UtcNow;
                    break;
            }
        }

        return base.SaveChangesAsync(cancellation);
    }
}
