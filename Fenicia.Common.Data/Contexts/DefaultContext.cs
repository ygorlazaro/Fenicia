using System.Reflection;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Common.Data.Contexts;

public partial class DefaultContext: DbContext
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
            entry.Entity.CompanyId = this.CurrentCompanyId switch
            {
                null => throw new InvalidOperationException("CompanyId is required"),
                _ => this.CurrentCompanyId.Value
            };

        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        PostgresDateTimeOffsetSupport.Init(modelBuilder);
        ApplyFilters(modelBuilder);

        modelBuilder.Entity<Models.Basic.CustomerModel>()
            .HasOne(c => c.Person)
            .WithOne(p => p.Customer)
            .HasForeignKey<Models.Basic.CustomerModel>(c => c.PersonId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Models.Basic.EmployeeModel>()
            .HasOne(e => e.Person)
            .WithOne(p => p.Employee)
            .HasForeignKey<Models.Basic.EmployeeModel>(e => e.PersonId)
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
                    .GetMethod(nameof(SetFilter),
                        BindingFlags.NonPublic | BindingFlags.Instance)!
                    .MakeGenericMethod(entityType.ClrType);

                method.Invoke(this,
                [
                    modelBuilder
                ]);
            }
            else if (typeof(BaseModel).IsAssignableFrom(entityType.ClrType) &&
                     !typeof(BaseCompanyModel).IsAssignableFrom(entityType.ClrType))
            {
                var method = typeof(DefaultContext)
                    .GetMethod(nameof(SetSoftDeleteFilter),
                        BindingFlags.NonPublic | BindingFlags.Instance)!
                    .MakeGenericMethod(entityType.ClrType);

                method.Invoke(this,
                [
                    modelBuilder
                ]);
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
