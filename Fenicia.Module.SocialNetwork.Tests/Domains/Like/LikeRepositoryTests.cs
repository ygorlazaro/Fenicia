using AwesomeAssertions;
using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.SocialNetwork;
using Fenicia.Common.Tests;
using Fenicia.Module.SocialNetwork.Domains.Like;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.SocialNetwork.Tests.Domains.Like;

public class LikeRepositoryTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly LikeRepository _repository;

    public LikeRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        _db = new DefaultContext(options, companyContext);
        _repository = new LikeRepository(_db);
        _faker = new Faker();
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }
}
