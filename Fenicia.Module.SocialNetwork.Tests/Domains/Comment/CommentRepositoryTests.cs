using AwesomeAssertions;
using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.SocialNetwork;
using Fenicia.Common.Tests;
using Fenicia.Module.SocialNetwork.Domains.Comment;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.SocialNetwork.Tests.Domains.Comment;

public class CommentRepositoryTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly CommentRepository _repository;

    public CommentRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        _db = new DefaultContext(options, companyContext);
        _repository = new CommentRepository(_db);
        _faker = new Faker();
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }
}
