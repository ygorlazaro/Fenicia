using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Tests.Models;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Common.Data.Tests.Contexts;

public class TestDataContext(DbContextOptions<DefaultContext> options, ICompanyContext companyContext)
    : DefaultContext(options, companyContext)
{
    public DbSet<TestEntity> TestEntities => Set<TestEntity>();
}
