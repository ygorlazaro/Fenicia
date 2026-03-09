using System.Reflection;

using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Data.Models.ProjectModels;
using Fenicia.Common.Data.Models.SocialNetworkModels;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

using OrderDetailModel = Fenicia.Common.Data.Models.Basic.OrderDetailModel;
using OrderModel = Fenicia.Common.Data.Models.Basic.OrderModel;

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

    public DbSet<RoleModel> Roles { get; set; } = null!;

    public DbSet<UserModel> AuthUsers { get; set; } = null!;

    public DbSet<UserRoleModel> UserRoles { get; set; } = null!;

    public DbSet<CompanyModel> Companies { get; set; } = null!;

    public DbSet<ModuleModel> Modules { get; set; } = null!;

    public DbSet<Models.Auth.OrderModel> Orders { get; set; } = null!;

    public DbSet<Models.Auth.OrderDetailModel> OrderDetails { get; set; } = null!;

    public DbSet<SubscriptionModel> Subscriptions { get; set; } = null!;

    public DbSet<SubscriptionCreditModel> SubscriptionCredits { get; set; } = null!;

    public DbSet<AddressModel> Addresses { get; set; } = null!;

    public DbSet<StateModel> States { get; set; } = null!;

    public DbSet<ForgotPasswordModel> ForgottenPasswords { get; set; } = null!;

    public DbSet<SubmoduleModel> Submodules { get; set; } = null!;

    public DbSet<CustomerModel> BasicCustomers { get; set; }

    public DbSet<EmployeeModel> BasicEmployees { get; set; }

    public DbSet<PositionModel> BasicPositions { get; set; }

    public DbSet<ProductCategoryModel> BasicProductCategories { get; set; }

    public DbSet<ProductModel> BasicProducts { get; set; }

    public DbSet<StockMovementModel> BasicStockMovements { get; set; }

    public DbSet<SupplierModel> BasicSuppliers { get; set; }

    public DbSet<OrderModel> BasicOrders { get; set; }

    public DbSet<OrderDetailModel> BasicOrderDetails { get; set; }

    public DbSet<PersonModel> BasicPeople { get; set; }

    public DbSet<FeedModel> SNFeeds { get; set; }

    public DbSet<FollowerModel> SNFollowers { get; set; }

    public DbSet<ProjectModel> Projects { get; set; }

    public DbSet<ProjectStatusModel> ProjectStatuses { get; set; }

    public DbSet<ProjectTaskModel> ProjectTasks { get; set; }

    public DbSet<ProjectSubtaskModel> ProjectSubtasks { get; set; }

    public DbSet<ProjectCommentModel> ProjectComments { get; set; }

    public DbSet<AttachmentModel> ProjectAttachments { get; set; }

    public DbSet<TaskAssigneeModel> ProjectTaskAssignees { get; set; }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken)
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
                    item.State = EntityState.Modified;
                    break;
            }
        }

        ApplyCompanyId();

        return base.SaveChangesAsync(cancellationToken);
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
        ApplyFilters(modelBuilder);

        modelBuilder.Entity<CustomerModel>()
            .HasOne(c => c.Person)
            .WithOne(p => p.Customer)
            .HasForeignKey<CustomerModel>(c => c.PersonId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<EmployeeModel>()
            .HasOne(e => e.Person)
            .WithOne(p => p.Employee)
            .HasForeignKey<EmployeeModel>(e => e.PersonId)
            .OnDelete(DeleteBehavior.Cascade);

        base.OnModelCreating(modelBuilder);
    }

    private void ApplyFilters(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseCompanyModel).IsAssignableFrom(entityType.ClrType))
            {
                var method = typeof(DefaultContext)
                    .GetMethod(nameof(SetFilter), BindingFlags.NonPublic | BindingFlags.Instance)!
                    .MakeGenericMethod(entityType.ClrType);

                method.Invoke(this, [modelBuilder]);
            }
            else if (typeof(BaseModel).IsAssignableFrom(entityType.ClrType) &&
                     !typeof(BaseCompanyModel).IsAssignableFrom(entityType.ClrType))
            {
                var method = typeof(DefaultContext)
                    .GetMethod(nameof(SetSoftDeleteFilter), BindingFlags.NonPublic | BindingFlags.Instance)!
                    .MakeGenericMethod(entityType.ClrType);

                method.Invoke(this, [modelBuilder]);
            }
        }
    }

    private void SetFilter<TEntity>(ModelBuilder modelBuilder)
        where TEntity : BaseCompanyModel
    {
        modelBuilder.Entity<TEntity>()
            .HasQueryFilter(e => (this.CurrentCompanyId == null || e.CompanyId == this.CurrentCompanyId)
                              && e.Deleted == null);
    }

    private void SetSoftDeleteFilter<TEntity>(ModelBuilder modelBuilder)
        where TEntity : BaseModel
    {
        modelBuilder.Entity<TEntity>()
            .HasQueryFilter(e => e.Deleted == null);
    }
}
