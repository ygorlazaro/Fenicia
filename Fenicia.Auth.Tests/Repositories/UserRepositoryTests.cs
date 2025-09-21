namespace Fenicia.Auth.Tests.Repositories;

using Bogus;

using Common.Database.Contexts;
using Fenicia.Common.Database.Models.Auth;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Moq;
using Domains.User;

public class UserRepositoryTests
{
    private readonly CancellationToken cancellationToken = CancellationToken.None;
    private Faker<CompanyModel> companyGenerator;
    private AuthContext context;
    private DbContextOptions<AuthContext> options;
    private UserRepository sut;
    private Faker<UserModel> userGenerator;
    private Faker<UserRoleModel> userRoleGenerator;

    [SetUp]
    public void Setup()
    {
        var mockLogger = new Mock<ILogger<UserRepository>>().Object;
        this.options = new DbContextOptionsBuilder<AuthContext>().UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}").Options;

        this.context = new AuthContext(this.options);
        this.sut = new UserRepository(this.context, mockLogger);

        this.SetupFakers();
    }

    [Test]
    public async Task GetByEmailAndCnpjAsyncWhenUserExistsReturnsUser()
    {
        // Arrange
        var user = this.userGenerator.Generate();
        var company = this.companyGenerator.Generate();
        var userRole = this.userRoleGenerator.Clone().RuleFor(ur => ur.UserId, user.Id).RuleFor(ur => ur.CompanyId, company.Id).Generate();

        await this.context.Users.AddAsync(user, this.cancellationToken);
        await this.context.Companies.AddAsync(company, this.cancellationToken);
        await this.context.UserRoles.AddAsync(userRole, this.cancellationToken);
        await this.context.SaveChangesAsync(this.cancellationToken);

        // Act
        var result = await this.sut.GetByEmailAsync(user.Email, this.cancellationToken);

        // Assert
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
        // Arrange
        const string nonExistentEmail = "nonexistent@example.com";

        // Act
        var result = await this.sut.GetByEmailAsync(nonExistentEmail, this.cancellationToken);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public void AddShouldAddUserToContext()
    {
        // Arrange
        var user = this.userGenerator.Generate();

        // Act
        var result = this.sut.Add(user);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(this.context.Users.Local, Does.Contain(user));
        }
    }

    [Test]
    public async Task SaveAsyncShouldPersistChangesToDatabase()
    {
        // Arrange
        var user = this.userGenerator.Generate();
        this.sut.Add(user);

        // Act
        var saveResult = await this.sut.SaveAsync(this.cancellationToken);

        // Assert
        Assert.That(saveResult, Is.GreaterThan(expected: 0));
        var savedUser = await this.context.Users.FirstOrDefaultAsync(x => x.Id == user.Id, this.cancellationToken);
        Assert.That(savedUser, Is.Not.Null);
        Assert.That(savedUser!.Email, Is.EqualTo(user.Email));
    }

    [Test]
    public async Task CheckUserExistsAsyncWhenUserExistsReturnsTrue()
    {
        // Arrange
        var user = this.userGenerator.Generate();
        await this.context.Users.AddAsync(user, this.cancellationToken);
        await this.context.SaveChangesAsync(this.cancellationToken);

        // Act
        var result = await this.sut.CheckUserExistsAsync(user.Email, this.cancellationToken);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task CheckUserExistsAsyncWhenUserDoesNotExistReturnsFalse()
    {
        // Arrange
        var nonExistentEmail = "nonexistent@example.com";

        // Act
        var result = await this.sut.CheckUserExistsAsync(nonExistentEmail, this.cancellationToken);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task GetUserForRefreshTokenAsyncWhenUserExistsReturnsUser()
    {
        // Arrange
        var user = this.userGenerator.Generate();
        await this.context.Users.AddAsync(user, this.cancellationToken);
        await this.context.SaveChangesAsync(this.cancellationToken);

        // Act
        var result = await this.sut.GetUserForRefreshTokenAsync(user.Id, this.cancellationToken);

        // Assert
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
        // Arrange
        var nonExistentUserId = Guid.NewGuid();

        // Act
        var result = await this.sut.GetUserForRefreshTokenAsync(nonExistentUserId, this.cancellationToken);

        // Assert
        Assert.That(result, Is.Null);
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Database.EnsureDeleted();
        this.context.Dispose();
    }

    private void SetupFakers()
    {
        this.userGenerator = new Faker<UserModel>().RuleFor(u => u.Id, _ => Guid.NewGuid()).RuleFor(u => u.Email, f => f.Internet.Email()).RuleFor(u => u.Name, f => f.Name.FullName()).RuleFor(u => u.Password, f => f.Internet.Password());

        this.companyGenerator = new Faker<CompanyModel>().RuleFor(c => c.Id, _ => Guid.NewGuid()).RuleFor(c => c.Name, f => f.Company.CompanyName()).RuleFor(c => c.Cnpj, f => f.Random.ReplaceNumbers("##.###.###/####-##"));

        this.userRoleGenerator = new Faker<UserRoleModel>().RuleFor(ur => ur.Id, _ => Guid.NewGuid()).RuleFor(ur => ur.UserId, _ => Guid.NewGuid()).RuleFor(ur => ur.CompanyId, _ => Guid.NewGuid());
    }
}
