using Bogus;

using Fenicia.Auth.Domains.User;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;

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
        this.options = new DbContextOptionsBuilder<AuthContext>().UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}").Options;

        this.context = new AuthContext(this.options);
        this.sut = new UserRepository(this.context);

        SetupFakers();
    }

    [Test]
    public async Task GetByEmailAndCnpjAsyncWhenUserExistsReturnsUser()
    {
        var user = this.userGenerator.Generate();
        var company = this.companyGenerator.Generate();
        var userRole = this.userRoleGenerator.Clone().RuleFor(ur => ur.UserId, user.Id).RuleFor(ur => ur.CompanyId, company.Id).Generate();

        await this.context.Users.AddAsync(user, this.cancellationToken);
        await this.context.Companies.AddAsync(company, this.cancellationToken);
        await this.context.UserRoles.AddAsync(userRole, this.cancellationToken);
        await this.context.SaveChangesAsync(this.cancellationToken);

        var result = await this.sut.GetByEmailAsync(user.Email, this.cancellationToken);

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

        var result = await this.sut.GetByEmailAsync(nonExistentEmail, this.cancellationToken);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task SaveAsyncShouldPersistChangesToDatabase()
    {
        var user = this.userGenerator.Generate();
        this.sut.Add(user);

        var saveResult = await this.sut.SaveChangesAsync(this.cancellationToken);

        Assert.That(saveResult, Is.GreaterThan(0));
        var savedUser = await this.context.Users.FirstOrDefaultAsync(x => x.Id == user.Id, this.cancellationToken);
        Assert.That(savedUser, Is.Not.Null);
        Assert.That(savedUser!.Email, Is.EqualTo(user.Email));
    }

    [Test]
    public async Task CheckUserExistsAsyncWhenUserExistsReturnsTrue()
    {
        var user = this.userGenerator.Generate();
        await this.context.Users.AddAsync(user, this.cancellationToken);
        await this.context.SaveChangesAsync(this.cancellationToken);

        var result = await this.sut.CheckUserExistsAsync(user.Email, this.cancellationToken);

        Assert.That(result, Is.True);
    }

    [Test]
    public async Task CheckUserExistsAsyncWhenUserDoesNotExistReturnsFalse()
    {
        var nonExistentEmail = "nonexistent@example.com";

        var result = await this.sut.CheckUserExistsAsync(nonExistentEmail, this.cancellationToken);

        Assert.That(result, Is.False);
    }

    [Test]
    public async Task GetUserForRefreshTokenAsyncWhenUserExistsReturnsUser()
    {
        var user = this.userGenerator.Generate();
        await this.context.Users.AddAsync(user, this.cancellationToken);
        await this.context.SaveChangesAsync(this.cancellationToken);

        var result = await this.sut.GetByIdAsync(user.Id, this.cancellationToken);

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

        var result = await this.sut.GetByIdAsync(nonExistentUserId, this.cancellationToken);

        Assert.That(result, Is.Null);
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Database.EnsureDeleted();
        this.context.Dispose();
    }

    [Test]
    public async Task GetUserIdFromEmailAsyncWhenUserExistsReturnsId()
    {
        var user = this.userGenerator.Generate();
        await this.context.Users.AddAsync(user, this.cancellationToken);
        await this.context.SaveChangesAsync(this.cancellationToken);

        var result = await this.sut.GetUserIdFromEmailAsync(user.Email, this.cancellationToken);

        Assert.That(result, Is.EqualTo(user.Id));
    }

    [Test]
    public async Task GetByIdAsyncWhenUserExistsReturnsUser()
    {
        var user = this.userGenerator.Generate();
        await this.context.Users.AddAsync(user, this.cancellationToken);
        await this.context.SaveChangesAsync(this.cancellationToken);

        var result = await this.sut.GetByIdAsync(user.Id, this.cancellationToken);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Id, Is.EqualTo(user.Id));
    }

    [Test]
    public async Task UpdateShouldMarkEntityModifiedAndSavePersistsChanges()
    {
        var user = this.userGenerator.Generate();
        await this.context.Users.AddAsync(user, this.cancellationToken);
        await this.context.SaveChangesAsync(this.cancellationToken);

        user.Password = "newpass";
        this.sut.Update(user);
        await this.sut.SaveChangesAsync(this.cancellationToken);

        var saved = await this.context.Users.FirstOrDefaultAsync(u => u.Id == user.Id, this.cancellationToken);
        Assert.That(saved, Is.Not.Null);
        Assert.That(saved!.Password, Is.EqualTo("newpass"));
    }

    private void SetupFakers()
    {
        this.userGenerator = new Faker<UserModel>().RuleFor(u => u.Id, _ => Guid.NewGuid()).RuleFor(u => u.Email, f => f.Internet.Email()).RuleFor(u => u.Name, f => f.Name.FullName()).RuleFor(u => u.Password, f => f.Internet.Password());

        this.companyGenerator = new Faker<CompanyModel>().RuleFor(c => c.Id, _ => Guid.NewGuid()).RuleFor(c => c.Name, f => f.Company.CompanyName()).RuleFor(c => c.Cnpj, f => f.Random.ReplaceNumbers("##.###.###/####-##"));

        this.userRoleGenerator = new Faker<UserRoleModel>().RuleFor(ur => ur.Id, _ => Guid.NewGuid()).RuleFor(ur => ur.UserId, _ => Guid.NewGuid()).RuleFor(ur => ur.CompanyId, _ => Guid.NewGuid());
    }
}