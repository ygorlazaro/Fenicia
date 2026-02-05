using Fenicia.Auth.Domains.ForgotPassword;
using Fenicia.Common.Database.Contexts;
using Fenicia.Common.Database.Models.Auth;
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
        // Arrange
        var model = new ForgotPasswordModel { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), Code = "ABC123", IsActive = true, ExpirationDate = DateTime.UtcNow.AddDays(1) };
        await context.ForgottenPasswords.AddAsync(model, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        // Act
        var result = await sut.GetFromUserIdAndCodeAsync(model.UserId, model.Code, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Id, Is.EqualTo(model.Id));
    }

    [Test]
    public async Task InvalidateCodeAsyncDoesNothingWhenNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        await sut.InvalidateCodeAsync(id, cancellationToken);

        // Assert - should not throw and DB empty
        Assert.That(await context.ForgottenPasswords.CountAsync(cancellationToken), Is.EqualTo(0));
    }

    [Test]
    public async Task InvalidateCodeAsyncSetsIsActiveFalseWhenFound()
    {
        // Arrange
        var model = new ForgotPasswordModel { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), Code = "ABC123", IsActive = true, ExpirationDate = DateTime.UtcNow.AddDays(1) };
        await context.ForgottenPasswords.AddAsync(model, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        // Act
        await sut.InvalidateCodeAsync(model.Id, cancellationToken);

        // Assert
        var fromDb = await context.ForgottenPasswords.FirstOrDefaultAsync(fp => fp.Id == model.Id, cancellationToken);
        Assert.That(fromDb, Is.Not.Null);
        Assert.That(fromDb!.IsActive, Is.False);
    }

    [Test]
    public async Task SaveForgotPasswordAsyncAddsAndReturnsModel()
    {
        // Arrange
        var model = new ForgotPasswordModel { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), Code = "XYZ789", IsActive = true };

        // Act
        var result = await sut.SaveForgotPasswordAsync(model, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(model.Id));
        var fromDb = await context.ForgottenPasswords.FindAsync(new object[] { model.Id }, cancellationToken);
        Assert.That(fromDb, Is.Not.Null);
    }
}
