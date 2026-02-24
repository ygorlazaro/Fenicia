using Fenicia.Common.Data.Models.Auth;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Common.Data.Contexts;

public class AuthContext(DbContextOptions<AuthContext> options) : DbContext(options)
{
    public AuthContext() : this(new DbContextOptions<AuthContext>())
    {

    }

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

    public DbSet<ForgotPasswordModel> ForgottenPasswords { get; set; } = null!;

    public DbSet<SubmoduleModel> Submodules { get; set; } = null!;

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