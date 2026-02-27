using Fenicia.Common.Data.Models;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Common.Data.Contexts;

public class DefaultContext(DbContextOptions<DefaultContext> options) : DbContext(options)
{
    public DefaultContext() : this(new DbContextOptions<DefaultContext>())
    {

    }

    public DbSet<AuthRole> Roles { get; set; } = null!;

    public DbSet<AuthUser> AuthUsers { get; set; } = null!;

    public DbSet<AuthUserRole> UserRoles { get; set; } = null!;

    public DbSet<AuthCompany> Companies { get; set; } = null!;

    public DbSet<AuthModule> Modules { get; set; } = null!;

    public DbSet<AuthOrder> Orders { get; set; } = null!;

    public DbSet<AuthOrderDetail> OrderDetails { get; set; } = null!;

    public DbSet<AuthSubscription> Subscriptions { get; set; } = null!;

    public DbSet<AuthSubscriptionCredit> SubscriptionCredits { get; set; } = null!;

    public DbSet<AuthAddress> Addresses { get; set; } = null!;

    public DbSet<AuthState> States { get; set; } = null!;

    public DbSet<AuthForgotPassowrd> ForgottenPasswords { get; set; } = null!;

    public DbSet<AuthSubmodule> Submodules { get; set; } = null!;

    public DbSet<BasicCustomer> BasicCustomers { get; set; }

    public DbSet<BasicEmployee> BasicEmployees { get; set; }

    public DbSet<BasicPosition> BasicPositions { get; set; }

    public DbSet<BasicProductCategory> BasicProductCategories { get; set; }

    public DbSet<BasicProduct> BasicProducts { get; set; }

    public DbSet<BasicStockMovement> BasicStockMovements { get; set; }

    public DbSet<BasicSupplier> BasicSuppliers { get; set; }

    public DbSet<BasicOrder> BasicOrders { get; set; }

    public DbSet<BasicOrderDetail> BasicOrderDetails { get; set; }

    public DbSet<BasicPerson> BasicPeople { get; set; }

    public DbSet<SNFeed> SNFeeds { get; set; }

    public DbSet<SNFollower> SNFollowers { get; set; }

    public override Task<int> SaveChangesAsync(CancellationToken ct)
    {
        foreach (var item in this.ChangeTracker.Entries())
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

        return base.SaveChangesAsync(ct);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        PostgresDateTimeOffsetSupport.Init(modelBuilder);
        SoftDeleteQueryExtension.AddSoftDeleteSupport(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }
}
