using Fenicia.Auth.Domains.ForgotPassword;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Repositories;

public class ForgotPasswordRepositoryTests
{
    private DbContextOptions<AuthContext> options;
    private AuthContext context;
    private ForgotPasswordRepository sut;
    private CancellationToken cancellationToken;

    [SetUp]
    public void Setup()
    {
        options = new DbContextOptionsBuilder<AuthContext>().UseInMemoryDatabase($"TestDb_FP_{Guid.NewGuid()}").Options;
        context = new AuthContext(options);
        sut = new ForgotPasswordRepository(context);
        cancellationToken = CancellationToken.None;
    }

    [TearDown]
    public void TearDown()
    {
        context.Database.EnsureDeleted();
        context.Dispose();
    }

    [Test]
    public async Task GetFromUserIdAndCodeAsyncReturnsWhenValid()
    {
        var model = new ForgotPasswordModel { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), Code = "ABC123", IsActive = true, ExpirationDate = DateTime.UtcNow.AddDays(1) };
        await context.ForgottenPasswords.AddAsync(model, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        var result = await sut.GetFromUserIdAndCodeAsync(model.UserId, model.Code, cancellationToken);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Id, Is.EqualTo(model.Id));
    }

    [Test]
    public async Task InvalidateCodeAsyncDoesNothingWhenNotFound()
    {
        var id = Guid.NewGuid();

        await sut.InvalidateCodeAsync(id, cancellationToken);

        Assert.That(await context.ForgottenPasswords.CountAsync(cancellationToken), Is.Zero);
    }

    [Test]
    public async Task InvalidateCodeAsyncSetsIsActiveFalseWhenFound()
    {
        var model = new ForgotPasswordModel { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), Code = "ABC123", IsActive = true, ExpirationDate = DateTime.UtcNow.AddDays(1) };
        await context.ForgottenPasswords.AddAsync(model, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        await sut.InvalidateCodeAsync(model.Id, cancellationToken);

        var fromDb = await context.ForgottenPasswords.FirstOrDefaultAsync(fp => fp.Id == model.Id, cancellationToken);
        Assert.That(fromDb, Is.Not.Null);
        Assert.That(fromDb!.IsActive, Is.False);
    }

    [Test]
    public async Task SaveForgotPasswordAsyncAddsAndReturnsModel()
    {
        var model = new ForgotPasswordModel { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), Code = "XYZ789", IsActive = true };

        var result = await sut.SaveForgotPasswordAsync(model, cancellationToken);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(model.Id));
        var fromDb = await context.ForgottenPasswords.FindAsync([model.Id], cancellationToken);
        Assert.That(fromDb, Is.Not.Null);
    }
}
