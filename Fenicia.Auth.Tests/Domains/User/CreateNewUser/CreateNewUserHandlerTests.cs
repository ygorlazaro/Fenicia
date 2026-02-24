using Bogus;
using Bogus.Extensions.Brazil;

using Fenicia.Auth.Domains.Company.CheckCompanyExists;
using Fenicia.Auth.Domains.Role.GetAdminRole;
using Fenicia.Auth.Domains.Security.HashPassword;
using Fenicia.Auth.Domains.User;
using Fenicia.Auth.Domains.User.CreateNewUser;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Enums.Auth;
using Fenicia.Common.Migrations.Services;

using Microsoft.EntityFrameworkCore;

using Moq;

namespace Fenicia.Auth.Tests.Domains.User.CreateNewUser;

[TestFixture]
public class CreateNewUserHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<AuthContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new AuthContext(options);
        this.checkUserExistsHandlerMock = new Mock<CheckUserExistsHandler>(this.context);
        this.checkCompanyExistsHandlerMock = new Mock<CheckCompanyExistsHandler>(this.context);
        this.hashPasswordHandler = new HashPasswordHandler();
        this.getAdminRoleHandlerMock = new Mock<GetAdminRoleHandler>(this.context);
        this.migrationServiceMock = new Mock<IMigrationService>();
        this.handler = new CreateNewUserHandler(
            this.context,
            this.checkUserExistsHandlerMock.Object,
            this.checkCompanyExistsHandlerMock.Object,
            this.hashPasswordHandler,
            this.getAdminRoleHandlerMock.Object,
            this.migrationServiceMock.Object
        );
        this.faker = new Faker();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private AuthContext context = null!;
    private CreateNewUserHandler handler = null!;
    private Mock<CheckUserExistsHandler> checkUserExistsHandlerMock = null!;
    private Mock<CheckCompanyExistsHandler> checkCompanyExistsHandlerMock = null!;
    private HashPasswordHandler hashPasswordHandler = null!;
    private Mock<GetAdminRoleHandler> getAdminRoleHandlerMock = null!;
    private Mock<IMigrationService> migrationServiceMock = null!;
    private Faker faker = null!;

    [Test]
    public async Task Handle_WhenValidRequest_CreatesUserAndCompanySuccessfully()
    {
        // Arrange
        var email = this.faker.Internet.Email();
        var password = this.faker.Internet.Password();
        var name = this.faker.Person.FullName;
        var cnpj = this.faker.Company.Cnpj();
        var companyName = this.faker.Company.CompanyName();
        var timeZone = "UTC";

        this.checkUserExistsHandlerMock
            .Setup(x => x.Handle(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        this.checkCompanyExistsHandlerMock
            .Setup(x => x.Handle(It.IsAny<CheckCompanyExistsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        this.getAdminRoleHandlerMock
            .Setup(x => x.Handle(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetAdminRoleResponse(Guid.NewGuid(), "Admin"));

        var request = new CreateNewUserQuery(email, password, name,
            new CreateNewUserCompanyQuery(cnpj, companyName, timeZone));

        // Act
        var result = await this.handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);

        var user = await this.context.Users.FirstOrDefaultAsync(u => u.Email == email);
        Assert.That(user, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(user!.Email, Is.EqualTo(email), "Email should match");
            Assert.That(user.Name, Is.EqualTo(name), "Name should match");
            Assert.That(user.Password, Is.Not.EqualTo(password), "Password should be hashed");
        }

        var company = await this.context.Companies.FirstOrDefaultAsync(c => c.Cnpj == cnpj);
        Assert.That(company, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(company!.Name, Is.EqualTo(companyName), "Company name should match");
            Assert.That(company.Cnpj, Is.EqualTo(cnpj), "CNPJ should match");
            Assert.That(company.TimeZone, Is.EqualTo(timeZone), "TimeZone should match");
        }
    }

    [Test]
    public void Handle_WhenEmailAlreadyExists_ThrowsArgumentException()
    {
        // Arrange
        var email = this.faker.Internet.Email();
        var password = this.faker.Internet.Password();
        var name = this.faker.Person.FullName;
        var cnpj = this.faker.Company.Cnpj();
        var companyName = this.faker.Company.CompanyName();
        var timeZone = "UTC";

        this.checkUserExistsHandlerMock
            .Setup(x => x.Handle(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var request = new CreateNewUserQuery(email, password, name,
            new CreateNewUserCompanyQuery(cnpj, companyName, timeZone));

        // Act & Assert
        var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
            await this.handler.Handle(request, CancellationToken.None)
        );
        Assert.That(ex?.Message, Is.EqualTo("This email exists"));
    }

    [Test]
    public void Handle_WhenCompanyAlreadyExists_ThrowsArgumentException()
    {
        // Arrange
        var email = this.faker.Internet.Email();
        var password = this.faker.Internet.Password();
        var name = this.faker.Person.FullName;
        var cnpj = this.faker.Company.Cnpj();
        var companyName = this.faker.Company.CompanyName();
        var timeZone = "UTC";

        this.checkUserExistsHandlerMock
            .Setup(x => x.Handle(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        this.checkCompanyExistsHandlerMock
            .Setup(x => x.Handle(It.IsAny<CheckCompanyExistsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var request = new CreateNewUserQuery(email, password, name,
            new CreateNewUserCompanyQuery(cnpj, companyName, timeZone));

        // Act & Assert
        var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
            await this.handler.Handle(request, CancellationToken.None)
        );
        Assert.That(ex?.Message, Is.EqualTo("This company exists"));
    }

    [Test]
    public void Handle_WhenAdminRoleNotFound_ThrowsArgumentException()
    {
        // Arrange
        var email = this.faker.Internet.Email();
        var password = this.faker.Internet.Password();
        var name = this.faker.Person.FullName;
        var cnpj = this.faker.Company.Cnpj();
        var companyName = this.faker.Company.CompanyName();
        var timeZone = "UTC";

        this.checkUserExistsHandlerMock
            .Setup(x => x.Handle(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        this.checkCompanyExistsHandlerMock
            .Setup(x => x.Handle(It.IsAny<CheckCompanyExistsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        this.getAdminRoleHandlerMock
            .Setup(x => x.Handle(It.IsAny<CancellationToken>()))
            .ReturnsAsync((GetAdminRoleResponse?)null);

        var request = new CreateNewUserQuery(email, password, name,
            new CreateNewUserCompanyQuery(cnpj, companyName, timeZone));

        // Act & Assert
        var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
            await this.handler.Handle(request, CancellationToken.None)
        );
        Assert.That(ex?.Message, Is.EqualTo("Missing admin role"));
    }

    [Test]
    public async Task Handle_CreatesUserRoleLinkingUserCompanyAndRole()
    {
        // Arrange
        var email = this.faker.Internet.Email();
        var password = this.faker.Internet.Password();
        var name = this.faker.Person.FullName;
        var cnpj = this.faker.Company.Cnpj();
        var companyName = this.faker.Company.CompanyName();
        var timeZone = "UTC";
        var adminRoleId = Guid.NewGuid();

        this.checkUserExistsHandlerMock
            .Setup(x => x.Handle(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        this.checkCompanyExistsHandlerMock
            .Setup(x => x.Handle(It.IsAny<CheckCompanyExistsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        this.getAdminRoleHandlerMock
            .Setup(x => x.Handle(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetAdminRoleResponse(adminRoleId, "Admin"));

        var request = new CreateNewUserQuery(email, password, name,
            new CreateNewUserCompanyQuery(cnpj, companyName, timeZone));

        // Act
        var result = await this.handler.Handle(request, CancellationToken.None);

        // Assert
        var userRole = await this.context.UserRoles.FirstOrDefaultAsync(ur => ur.UserId == result.Id);
        Assert.That(userRole, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(userRole!.RoleId, Is.EqualTo(adminRoleId), "RoleId should be Admin");
            Assert.That(userRole.CompanyId, Is.Not.EqualTo(Guid.Empty), "CompanyId should be set");
        }
    }

    [Test]
    public async Task Handle_CallsMigrationServiceForBasicModule()
    {
        // Arrange
        var email = this.faker.Internet.Email();
        var password = this.faker.Internet.Password();
        var name = this.faker.Person.FullName;
        var cnpj = this.faker.Company.Cnpj();
        var companyName = this.faker.Company.CompanyName();
        var timeZone = "UTC";

        this.checkUserExistsHandlerMock
            .Setup(x => x.Handle(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        this.checkCompanyExistsHandlerMock
            .Setup(x => x.Handle(It.IsAny<CheckCompanyExistsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        this.getAdminRoleHandlerMock
            .Setup(x => x.Handle(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetAdminRoleResponse(Guid.NewGuid(), "Admin"));

        var request = new CreateNewUserQuery(email, password, name,
            new CreateNewUserCompanyQuery(cnpj, companyName, timeZone));

        // Act
        await this.handler.Handle(request, CancellationToken.None);

        // Assert
        this.migrationServiceMock.Verify(
            x => x.RunMigrationsAsync(
                It.IsAny<Guid>(),
                It.Is<List<ModuleType>>(types => types.Contains(ModuleType.Basic)),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
    }

    [Test]
    public async Task Handle_ReturnsCorrectResponseData()
    {
        // Arrange
        var email = this.faker.Internet.Email();
        var password = this.faker.Internet.Password();
        var name = this.faker.Person.FullName;
        var cnpj = this.faker.Company.Cnpj();
        var companyName = this.faker.Company.CompanyName();
        var timeZone = "UTC";

        this.checkUserExistsHandlerMock
            .Setup(x => x.Handle(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        this.checkCompanyExistsHandlerMock
            .Setup(x => x.Handle(It.IsAny<CheckCompanyExistsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        this.getAdminRoleHandlerMock
            .Setup(x => x.Handle(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetAdminRoleResponse(Guid.NewGuid(), "Admin"));

        var request = new CreateNewUserQuery(email, password, name,
            new CreateNewUserCompanyQuery(cnpj, companyName, timeZone));

        // Act
        var result = await this.handler.Handle(request, CancellationToken.None);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.Not.EqualTo(Guid.Empty), "UserId should be set");
            Assert.That(result.Name, Is.EqualTo(name), "Name should match");
            Assert.That(result.Email, Is.EqualTo(email), "Email should match");
            Assert.That(result.Company.Id, Is.Not.EqualTo(Guid.Empty), "CompanyId should be set");
            Assert.That(result.Company.Name, Is.EqualTo(companyName), "CompanyName should match");
            Assert.That(result.Company.Cnpj, Is.EqualTo(cnpj), "CNPJ should match");
        }
    }

    [Test]
    public async Task Handle_PasswordIsHashedBeforeSaving()
    {
        // Arrange
        var email = this.faker.Internet.Email();
        var password = this.faker.Internet.Password();
        var name = this.faker.Person.FullName;
        var cnpj = this.faker.Company.Cnpj();
        var companyName = this.faker.Company.CompanyName();
        var timeZone = "UTC";

        this.checkUserExistsHandlerMock
            .Setup(x => x.Handle(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        this.checkCompanyExistsHandlerMock
            .Setup(x => x.Handle(It.IsAny<CheckCompanyExistsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        this.getAdminRoleHandlerMock
            .Setup(x => x.Handle(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetAdminRoleResponse(Guid.NewGuid(), "Admin"));

        var request = new CreateNewUserQuery(email, password, name,
            new CreateNewUserCompanyQuery(cnpj, companyName, timeZone));

        // Act
        await this.handler.Handle(request, CancellationToken.None);

        // Assert
        var user = await this.context.Users.FirstOrDefaultAsync(u => u.Email == email);
        Assert.That(user, Is.Not.Null);
        Assert.That(user!.Password, Is.Not.EqualTo(password), "Password should be hashed");
    }

    [Test]
    public async Task Handle_CompanyIsActiveByDefault()
    {
        // Arrange
        var email = this.faker.Internet.Email();
        var password = this.faker.Internet.Password();
        var name = this.faker.Person.FullName;
        var cnpj = this.faker.Company.Cnpj();
        var companyName = this.faker.Company.CompanyName();
        var timeZone = "UTC";

        this.checkUserExistsHandlerMock
            .Setup(x => x.Handle(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        this.checkCompanyExistsHandlerMock
            .Setup(x => x.Handle(It.IsAny<CheckCompanyExistsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        this.getAdminRoleHandlerMock
            .Setup(x => x.Handle(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetAdminRoleResponse(Guid.NewGuid(), "Admin"));

        var request = new CreateNewUserQuery(email, password, name,
            new CreateNewUserCompanyQuery(cnpj, companyName, timeZone));

        // Act
        await this.handler.Handle(request, CancellationToken.None);

        // Assert
        var company = await this.context.Companies.FirstOrDefaultAsync(c => c.Cnpj == cnpj);
        Assert.That(company, Is.Not.Null);
        Assert.That(company!.IsActive, Is.True, "Company should be active by default");
    }

    [Test]
    public async Task Handle_CompanyLanguageIsSetToDefault()
    {
        // Arrange
        var email = this.faker.Internet.Email();
        var password = this.faker.Internet.Password();
        var name = this.faker.Person.FullName;
        var cnpj = this.faker.Company.Cnpj();
        var companyName = this.faker.Company.CompanyName();
        var timeZone = "UTC";

        this.checkUserExistsHandlerMock
            .Setup(x => x.Handle(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        this.checkCompanyExistsHandlerMock
            .Setup(x => x.Handle(It.IsAny<CheckCompanyExistsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        this.getAdminRoleHandlerMock
            .Setup(x => x.Handle(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetAdminRoleResponse(Guid.NewGuid(), "Admin"));

        var request = new CreateNewUserQuery(email, password, name,
            new CreateNewUserCompanyQuery(cnpj, companyName, timeZone));

        // Act
        await this.handler.Handle(request, CancellationToken.None);

        // Assert
        var company = await this.context.Companies.FirstOrDefaultAsync(c => c.Cnpj == cnpj);
        Assert.That(company, Is.Not.Null);
        Assert.That(company!.Language, Is.EqualTo("pt-BR"), "Language should be pt-BR by default");
    }
}