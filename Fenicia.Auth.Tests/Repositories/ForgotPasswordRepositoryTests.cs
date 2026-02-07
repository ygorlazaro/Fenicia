using Fenicia.Auth.Domains.ForgotPassword;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Repositories;

public class ForgotPasswordRepositoryTests
{
    private CancellationToken cancellationToken;
    private AuthContext context;
    private DbContextOptions<AuthContext> options;
    private ForgotPasswordRepository sut;

    [SetUp]
    public void Setup()
    {
        this.options = new DbContextOptionsBuilder<AuthContext>().UseInMemoryDatabase($"TestDb_FP_{Guid.NewGuid()}")
            .Options;
        this.context = new AuthContext(this.options);
        this.sut = new ForgotPasswordRepository(this.context);
        this.cancellationToken = CancellationToken.None;
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Database.EnsureDeleted();
        this.context.Dispose();
    }

    [Test]
    public async Task GetFromUserIdAndCodeAsyncReturnsWhenValid()
    {
        var model = new ForgotPasswordModel
        {
            Id = Guid.NewGuid(), UserId = Guid.NewGuid(), Code = "ABC123", IsActive = true,
            ExpirationDate = DateTime.UtcNow.AddDays(1)
        };
        await this.context.ForgottenPasswords.AddAsync(model, this.cancellationToken);
        await this.context.SaveChangesAsync(this.cancellationToken);

        var result = await this.sut.GetFromUserIdAndCodeAsync(model.UserId, model.Code, this.cancellationToken);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Id, Is.EqualTo(model.Id));
    }

    [Test]
    public async Task InvalidateCodeAsyncDoesNothingWhenNotFound()
    {
        var id = Guid.NewGuid();

        await this.sut.InvalidateCodeAsync(id, this.cancellationToken);

        Assert.That(await this.context.ForgottenPasswords.CountAsync(this.cancellationToken), Is.Zero);
    }

    [Test]
    public async Task InvalidateCodeAsyncSetsIsActiveFalseWhenFound()
    {
        var model = new ForgotPasswordModel
        {
            Id = Guid.NewGuid(), UserId = Guid.NewGuid(), Code = "ABC123", IsActive = true,
            ExpirationDate = DateTime.UtcNow.AddDays(1)
        };
        await this.context.ForgottenPasswords.AddAsync(model, this.cancellationToken);
        await this.context.SaveChangesAsync(this.cancellationToken);

        await this.sut.InvalidateCodeAsync(model.Id, this.cancellationToken);

        var fromDb =
            await this.context.ForgottenPasswords.FirstOrDefaultAsync(fp => fp.Id == model.Id, this.cancellationToken);
        Assert.That(fromDb, Is.Not.Null);
        Assert.That(fromDb!.IsActive, Is.False);
    }

    [Test]
    public async Task SaveForgotPasswordAsyncAddsAndReturnsModel()
    {
        var model = new ForgotPasswordModel
            { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), Code = "XYZ789", IsActive = true };

        var result = await this.sut.SaveForgotPasswordAsync(model, this.cancellationToken);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(model.Id));
        var fromDb = await this.context.ForgottenPasswords.FindAsync([model.Id], this.cancellationToken);
        Assert.That(fromDb, Is.Not.Null);
    }
}