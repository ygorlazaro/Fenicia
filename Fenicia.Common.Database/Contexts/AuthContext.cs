namespace Fenicia.Common.Database.Contexts;

using Database;

using Models.Auth;

using Microsoft.EntityFrameworkCore;

public class AuthContext : DbContext
{
    public AuthContext(DbContextOptions<AuthContext> options)
        : base(options)
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

    public DbSet<RefreshTokenModel> RefreshTokens { get; set; } = null!;

    public override Task<int> SaveChangesAsync(CancellationToken cancellation = default)
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

        return base.SaveChangesAsync(cancellation);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        PostgresDateTimeOffsetSupport.Init(modelBuilder);
        AuthContext.AddSoftDeleteSupport(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }

    private static void AddSoftDeleteSupport(ModelBuilder modelBuilder)
    {
        var mutableEntityTypes = modelBuilder.Model.GetEntityTypes().Where(entityType => typeof(BaseModel).IsAssignableFrom(entityType.ClrType));

        foreach (var entityType in mutableEntityTypes)
        {
            entityType.AddSoftDeleteQueryFilter();
        }
    }
}
