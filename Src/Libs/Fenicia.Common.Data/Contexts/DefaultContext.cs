using System.Reflection;

using Fenicia.Common.Data.Models.Basic;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Common.Data.Contexts;

#pragma warning disable SA1601 // Partial elements should be documented
public partial class DefaultContext : DbContext
#pragma warning restore SA1601 // Partial elements should be documented
{
    private readonly ICompanyContext _companyContext;

    public DefaultContext(DbContextOptions<DefaultContext> options, ICompanyContext companyContext)
        : base(options)
    {
        this._companyContext = companyContext;
    }

    public DefaultContext()
        : base(new DbContextOptionsBuilder<DefaultContext>().Options)
    {
        _companyContext = new CompanyContext(new HttpContextAccessor());
    }

    public Guid? CurrentCompanyId => _companyContext.CompanyId;

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken)
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
                    item.State = EntityState.Modified;
                    break;
            }
        }

        ApplyCompanyId();

        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        PostgresDateTimeOffsetSupport.Init(modelBuilder);
        ApplyFilters(modelBuilder);

        modelBuilder.Entity<CustomerModel>().HasOne(c => c.Person).WithOne(p => p.Customer).HasForeignKey<CustomerModel>(c => c.PersonId).OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<EmployeeModel>().HasOne(e => e.Person).WithOne(p => p.Employee).HasForeignKey<EmployeeModel>(e => e.PersonId).OnDelete(DeleteBehavior.Cascade);

        base.OnModelCreating(modelBuilder);
    }

    private void ApplyCompanyId()
    {
        var entries = ChangeTracker.Entries<BaseCompanyModel>().Where(e => e.State == EntityState.Added && e.Entity.CompanyId == Guid.Empty);

        foreach (var entry in entries)
        {
            entry.Entity.CompanyId = CurrentCompanyId switch
            {
                null => throw new InvalidOperationException("CompanyId is required"),
                _ => CurrentCompanyId.Value
            };
        }
    }

    private void ApplyFilters(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseCompanyModel).IsAssignableFrom(entityType.ClrType))
            {
                var method = typeof(DefaultContext).GetMethod(nameof(SetFilter), BindingFlags.NonPublic | BindingFlags.Instance)!.MakeGenericMethod(entityType.ClrType);

                method.Invoke(this, [modelBuilder]);
            }
            else if (typeof(BaseModel).IsAssignableFrom(entityType.ClrType) && !typeof(BaseCompanyModel).IsAssignableFrom(entityType.ClrType))
            {
                var method = typeof(DefaultContext).GetMethod(nameof(SetSoftDeleteFilter), BindingFlags.NonPublic | BindingFlags.Instance)!.MakeGenericMethod(entityType.ClrType);

                method.Invoke(this, [modelBuilder]);
            }
        }
    }

    private void SetFilter<TEntity>(ModelBuilder modelBuilder)
        where TEntity : BaseCompanyModel
    {
        modelBuilder.Entity<TEntity>().HasQueryFilter(e => (CurrentCompanyId == null || e.CompanyId == CurrentCompanyId) && e.Deleted == null);
    }

    private void SetSoftDeleteFilter<TEntity>(ModelBuilder modelBuilder)
        where TEntity : BaseModel
    {
        modelBuilder.Entity<TEntity>().HasQueryFilter(e => e.Deleted == null);
    }
}
