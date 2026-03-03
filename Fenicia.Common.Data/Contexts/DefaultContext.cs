using System.Reflection;

using Fenicia.Common.Data.Models;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Common.Data.Contexts;

public class DefaultContext : DbContext
{

    private readonly ICompanyContext companyContext;

    public Guid? CurrentCompanyId => this.companyContext.CompanyId;

    public DefaultContext(DbContextOptions<DefaultContext> options, ICompanyContext companyContext) : base(options)
    {
        this.companyContext = companyContext;
    }

    public DefaultContext() : base(new DbContextOptions<DefaultContext>())
    {
        this.companyContext = new CompanyContext(new HttpContextAccessor());
    }

    public DbSet<AuthRoleModel> Roles { get; set; } = null!;

    public DbSet<AuthUserModel> AuthUsers { get; set; } = null!;

    public DbSet<AuthUserRoleModel> UserRoles { get; set; } = null!;

    public DbSet<AuthCompanyModel> Companies { get; set; } = null!;

    public DbSet<AuthModuleModel> Modules { get; set; } = null!;

    public DbSet<AuthOrderModel> Orders { get; set; } = null!;

    public DbSet<AuthOrderDetailModel> OrderDetails { get; set; } = null!;

    public DbSet<AuthSubscriptionModel> Subscriptions { get; set; } = null!;

    public DbSet<AuthSubscriptionCreditModel> SubscriptionCredits { get; set; } = null!;

    public DbSet<AuthAddressModel> Addresses { get; set; } = null!;

    public DbSet<AuthStateModel> States { get; set; } = null!;

    public DbSet<AuthForgotPassowrdModel> ForgottenPasswords { get; set; } = null!;

    public DbSet<AuthSubmoduleModel> Submodules { get; set; } = null!;

    public DbSet<BasicCustomerModel> BasicCustomers { get; set; }

    public DbSet<BasicEmployeeModel> BasicEmployees { get; set; }

    public DbSet<BasicPositionModel> BasicPositions { get; set; }

    public DbSet<BasicProductCategoryModel> BasicProductCategories { get; set; }

    public DbSet<BasicProductModel> BasicProducts { get; set; }

    public DbSet<BasicStockMovementModel> BasicStockMovements { get; set; }

    public DbSet<BasicSupplierModel> BasicSuppliers { get; set; }

    public DbSet<BasicOrderModel> BasicOrders { get; set; }

    public DbSet<BasicOrderDetailModel> BasicOrderDetails { get; set; }

    public DbSet<BasicPersonModel> BasicPeople { get; set; }

    public DbSet<SNFeedModel> SNFeeds { get; set; }

    public DbSet<SNFollowerModel> SNFollowers { get; set; }

    public DbSet<ProjectModel> Projects { get; set; }

    public DbSet<ProjectStatusModel> ProjectStatuses { get; set; }

    public DbSet<ProjectTaskModel> ProjectTasks { get; set; }

    public DbSet<ProjectSubtaskModel> ProjectSubtasks { get; set; }

    public DbSet<ProjectCommentModel> ProjectComments { get; set; }

    public DbSet<ProjectAttachmentModel> ProjectAttachments { get; set; }

    public DbSet<ProjectTaskAssigneeModel> ProjectTaskAssignees { get; set; }

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

        ApplyCompanyId();

        return base.SaveChangesAsync(ct);
    }

    private void ApplyCompanyId()
    {
        var entries = this.ChangeTracker
            .Entries<BaseCompanyModel>()
            .Where(e => e.State == EntityState.Added);

        foreach (var entry in entries)
        {
            if (this.CurrentCompanyId == null)
            {
                throw new InvalidOperationException("CompanyId is required");
            }

            entry.Entity.CompanyId = this.CurrentCompanyId.Value;
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        PostgresDateTimeOffsetSupport.Init(modelBuilder);
        SoftDeleteQueryExtension.AddSoftDeleteSupport(modelBuilder);
        ApplyCompanyFilter(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }


    private void ApplyCompanyFilter(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseCompanyModel).IsAssignableFrom(entityType.ClrType))
            {
                var method = typeof(DefaultContext)
                    .GetMethod(nameof(SetCompanyFilter),
                        BindingFlags.NonPublic | BindingFlags.Instance)!
                    .MakeGenericMethod(entityType.ClrType);

                method.Invoke(this, new object[] { modelBuilder });
            }
        }
    }

    private void SetCompanyFilter<TEntity>(ModelBuilder modelBuilder)
        where TEntity : BaseCompanyModel
    {
        modelBuilder.Entity<TEntity>()
            .HasQueryFilter(e =>
                this.CurrentCompanyId == null ||
                e.CompanyId == this.CurrentCompanyId);
    }
}
