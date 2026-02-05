using Bogus;

using Fenicia.Auth.Domains.User;
using Fenicia.Common.Database.Contexts;
using Fenicia.Common.Database.Models.Auth;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Repositories;

public class UserRepositoryTests
{
    private readonly CancellationToken cancellationToken = CancellationToken.None;
    private Faker<CompanyModel> companyGenerator;
    private AuthContext context = null!;
    private DbContextOptions<AuthContext> options = null!;
    private UserRepository sut = null!;
    private Faker<UserModel> userGenerator;
    private Faker<UserRoleModel> userRoleGenerator;

    [SetUp]
    public void Setup()
    {
        options = new DbContextOptionsBuilder<AuthContext>().UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}").Options;

        context = new AuthContext(options);
        sut = new UserRepository(context);

        SetupFakers();
    }

    [Test]
    public async Task GetByEmailAndCnpjAsyncWhenUserExistsReturnsUser()
    {
        var user = userGenerator.Generate();
        var company = companyGenerator.Generate();
        var userRole = userRoleGenerator.Clone().RuleFor(ur => ur.UserId, user.Id).RuleFor(ur => ur.CompanyId, company.Id).Generate();

        await context.Users.AddAsync(user, cancellationToken);
        await context.Companies.AddAsync(company, cancellationToken);
        await context.UserRoles.AddAsync(userRole, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        var result = await sut.GetByEmailAsync(user.Email, cancellationToken);

        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result!.Id, Is.EqualTo(user.Id));
            Assert.That(result.Email, Is.EqualTo(user.Email));
        }
    }

    [Test]
    public async Task GetByEmailAndCnpjAsyncWhenUserDoesNotExistReturnsNull()
    {
        const string nonExistentEmail = "nonexistent@example.com";

        var result = await sut.GetByEmailAsync(nonExistentEmail, cancellationToken);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task SaveAsyncShouldPersistChangesToDatabase()
    {
        var user = userGenerator.Generate();
        sut.Add(user);

        var saveResult = await sut.SaveChangesAsync(cancellationToken);

        Assert.That(saveResult, Is.GreaterThan(0));
        var savedUser = await context.Users.FirstOrDefaultAsync(x => x.Id == user.Id, cancellationToken);
        Assert.That(savedUser, Is.Not.Null);
        Assert.That(savedUser!.Email, Is.EqualTo(user.Email));
    }

    [Test]
    public async Task CheckUserExistsAsyncWhenUserExistsReturnsTrue()
    {
        var user = userGenerator.Generate();
        await context.Users.AddAsync(user, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        var result = await sut.CheckUserExistsAsync(user.Email, cancellationToken);

        Assert.That(result, Is.True);
    }

    [Test]
    public async Task CheckUserExistsAsyncWhenUserDoesNotExistReturnsFalse()
    {
        var nonExistentEmail = "nonexistent@example.com";

        var result = await sut.CheckUserExistsAsync(nonExistentEmail, cancellationToken);

        Assert.That(result, Is.False);
    }

    [Test]
    public async Task GetUserForRefreshTokenAsyncWhenUserExistsReturnsUser()
    {
        var user = userGenerator.Generate();
        await context.Users.AddAsync(user, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        var result = await sut.GetByIdAsync(user.Id, cancellationToken);

        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result!.Id, Is.EqualTo(user.Id));
            Assert.That(result.Email, Is.EqualTo(user.Email));
        }
    }

    [Test]
    public async Task GetUserForRefreshTokenAsyncWhenUserDoesNotExistReturnsNull()
    {
        var nonExistentUserId = Guid.NewGuid();

        var result = await sut.GetByIdAsync(nonExistentUserId, cancellationToken);

        Assert.That(result, Is.Null);
    }

    [TearDown]
    public void TearDown()
    {
        context.Database.EnsureDeleted();
        context.Dispose();
    }

    [Test]
    public async Task GetUserIdFromEmailAsyncWhenUserExistsReturnsId()
    {
        var user = userGenerator.Generate();
        await context.Users.AddAsync(user, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        var result = await sut.GetUserIdFromEmailAsync(user.Email, cancellationToken);

        Assert.That(result, Is.EqualTo(user.Id));
    }

    [Test]
    public async Task GetByIdAsyncWhenUserExistsReturnsUser()
    {
        var user = userGenerator.Generate();
        await context.Users.AddAsync(user, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        var result = await sut.GetByIdAsync(user.Id, cancellationToken);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Id, Is.EqualTo(user.Id));
    }

    [Test]
    public async Task UpdateShouldMarkEntityModifiedAndSavePersistsChanges()
    {
        var user = userGenerator.Generate();
        await context.Users.AddAsync(user, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        user.Password = "newpass";
        sut.Update(user);
        await sut.SaveChangesAsync(cancellationToken);

        var saved = await context.Users.FirstOrDefaultAsync(u => u.Id == user.Id, cancellationToken);
        Assert.That(saved, Is.Not.Null);
        Assert.That(saved!.Password, Is.EqualTo("newpass"));
    }

    private void SetupFakers()
    {
        userGenerator = new Faker<UserModel>().RuleFor(u => u.Id, _ => Guid.NewGuid()).RuleFor(u => u.Email, f => f.Internet.Email()).RuleFor(u => u.Name, f => f.Name.FullName()).RuleFor(u => u.Password, f => f.Internet.Password());

        companyGenerator = new Faker<CompanyModel>().RuleFor(c => c.Id, _ => Guid.NewGuid()).RuleFor(c => c.Name, f => f.Company.CompanyName()).RuleFor(c => c.Cnpj, f => f.Random.ReplaceNumbers("##.###.###/####-##"));

        userRoleGenerator = new Faker<UserRoleModel>().RuleFor(ur => ur.Id, _ => Guid.NewGuid()).RuleFor(ur => ur.UserId, _ => Guid.NewGuid()).RuleFor(ur => ur.CompanyId, _ => Guid.NewGuid());
    }
}
