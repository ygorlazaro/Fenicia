using Fenicia.Auth.Domains.Address;
using Fenicia.Auth.Domains.Company.Data;
using Fenicia.Auth.Domains.ForgotPassword.Data;
using Fenicia.Auth.Domains.Module.Data;
using Fenicia.Auth.Domains.Order.Data;
using Fenicia.Auth.Domains.OrderDetail.Data;
using Fenicia.Auth.Domains.RefreshToken.Data;
using Fenicia.Auth.Domains.Role.Data;
using Fenicia.Auth.Domains.State.Data;
using Fenicia.Auth.Domains.Subscription.Data;
using Fenicia.Auth.Domains.SubscriptionCredit.Data;
using Fenicia.Auth.Domains.User.Data;
using Fenicia.Auth.Domains.UserRole.Data;
using Fenicia.Common.Database;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Contexts;

/// <summary>
/// Database context for authentication and authorization related entities
/// </summary>
/// <param name="options">The options to be used by a DbContext</param>
public class AuthContext(DbContextOptions<AuthContext> options) : DbContext(options)
{
/// <summary>
/// Gets or sets the roles in the system
/// </summary>
public DbSet<RoleModel> Roles { get; set; } = null!;

/// <summary>
/// Gets or sets the users in the system
/// </summary>
public DbSet<UserModel> Users { get; set; } = null!;

/// <summary>
/// Gets or sets the user role assignments
/// </summary>
public DbSet<UserRoleModel> UserRoles { get; set; } = null!;

/// <summary>
/// Gets or sets the companies in the system
/// </summary>
public DbSet<CompanyModel> Companies { get; set; } = null!;

/// <summary>
/// Gets or sets the system modules
/// </summary>
public DbSet<ModuleModel> Modules { get; set; } = null!;

/// <summary>
/// Gets or sets the orders
/// </summary>
public DbSet<OrderModel> Orders { get; set; } = null!;

/// <summary>
/// Gets or sets the order details
/// </summary>
public DbSet<OrderDetailModel> OrderDetails { get; set; } = null!;

/// <summary>
/// Gets or sets the subscriptions
/// </summary>
public DbSet<SubscriptionModel> Subscriptions { get; set; } = null!;

/// <summary>
/// Gets or sets the subscription credits
/// </summary>
public DbSet<SubscriptionCreditModel> SubscriptionCredits { get; set; } = null!;

/// <summary>
/// Gets or sets the addresses
/// </summary>
public DbSet<AddressModel> Addresses { get; set; } = null!;

/// <summary>
/// Gets or sets the states
/// </summary>
public DbSet<StateModel> States { get; set; } = null!;

/// <summary>
/// Gets or sets the forgotten passwords
/// </summary>
public DbSet<ForgotPasswordModel> ForgottenPasswords { get; set; } = null!;

/// <summary>
/// Gets or sets the refresh tokens
/// </summary>
    public DbSet<RefreshTokenModel> RefreshTokens { get; set; } = null!;

    /// <summary>
    /// Configures the model that was discovered by convention from the entity types
    /// </summary>
    /// <param name="modelBuilder">The builder being used to construct the model for this context</param>
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

    /// <summary>
    /// Saves all changes made in this context to the database asynchronously
    /// </summary>
    /// <param name="cancellation">A CancellationToken to observe while waiting for the task to complete</param>
    /// <returns>A task that represents the asynchronous save operation. The task result contains the number of state entries written to the database</returns>
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
